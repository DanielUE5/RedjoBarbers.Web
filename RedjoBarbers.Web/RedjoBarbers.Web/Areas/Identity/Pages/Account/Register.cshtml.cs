#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RedjoBarbers.Web.Data.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace RedjoBarbers.Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Моля, въведете име.")]
            [StringLength(100, ErrorMessage = "Името не може да бъде по-дълго от 100 символа.")]
            [Display(Name = "Име")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Моля, въведете фамилия.")]
            [StringLength(100, ErrorMessage = "Фамилията не може да бъде по-дълга от 100 символа.")]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; } = string.Empty;

            [DataType(DataType.Date)]
            [Display(Name = "Дата на раждане (по избор)")]
            public DateTime? DateOfBirth { get; set; }

            [Required(ErrorMessage = "Моля, въведете имейл адрес.")]
            [EmailAddress(ErrorMessage = "Моля, въведете валиден имейл адрес.")]
            [Display(Name = "Имейл адрес")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Моля, въведете парола.")]
            [StringLength(100, ErrorMessage = "{0} трябва да е с дължина поне {2} и максимум {1} знака.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Парола")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Моля, потвърдете паролата.")]
            [DataType(DataType.Password)]
            [Display(Name = "Потвърдете паролата")]
            [Compare("Password", ErrorMessage = "Паролата и паролата за потвърждение не съвпадат.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                ApplicationUser user = CreateUser();

                user.FirstName = Input.FirstName.Trim();
                user.LastName = Input.LastName.Trim();
                user.DateOfBirth = Input.DateOfBirth;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                IdentityResult result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Потребител създаде нов акаунт.");

                    string adminEmail = _configuration["AdminSettings:Email"];

                    string roleName = string.Equals(Input.Email, adminEmail, StringComparison.OrdinalIgnoreCase)
                        ? "Admin"
                        : "User";

                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        IdentityResult createRoleResult = await _roleManager.CreateAsync(new ApplicationRole
                        {
                            Name = roleName,
                            NormalizedName = roleName.ToUpperInvariant(),
                            Label = roleName
                        });

                        if (!createRoleResult.Succeeded)
                        {
                            foreach (IdentityError error in createRoleResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }

                            return Page();
                        }
                    }

                    IdentityResult roleResult = await _userManager.AddToRoleAsync(user, roleName);

                    if (!roleResult.Succeeded)
                    {
                        foreach (IdentityError error in roleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return Page();
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Не може да се създаде екземпляр на '{nameof(ApplicationUser)}'.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("User store трябва да поддържа email.");
            }

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}