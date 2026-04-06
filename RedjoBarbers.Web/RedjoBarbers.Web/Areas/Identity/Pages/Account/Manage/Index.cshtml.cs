#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RedjoBarbers.Web.Data.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace RedjoBarbers.Web.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Моля, въведете име.")]
            [Display(Name = "Име")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Моля, въведете фамилия.")]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; } = string.Empty;

            [DataType(DataType.Date)]
            [Display(Name = "Дата на раждане (по избор)")]
            public DateTime? DateOfBirth { get; set; }

            [Phone(ErrorMessage = "Моля, въведете валиден телефонен номер.")]
            [Display(Name = "Телефонен номер (по избор)")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Моля, въведете имейл адрес.")]
            [EmailAddress(ErrorMessage = "Моля, въведете валиден имейл адрес.")]
            [Display(Name = "Имейл адрес")]
            public string Email { get; set; } = string.Empty;
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            string userName = await _userManager.GetUserNameAsync(user);
            string phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                Email = user.Email
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"User not found.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.FirstName = Input.FirstName.Trim();
            user.LastName = Input.LastName.Trim();
            user.DateOfBirth = Input.DateOfBirth;

            if (Input.Email != user.Email)
            {
                IdentityResult setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    StatusMessage = "Грешка при обновяване на имейла.";
                    return RedirectToPage();
                }

                IdentityResult setUserNameResult = await _userManager.SetUserNameAsync(user, Input.Email);
                if (!setUserNameResult.Succeeded)
                {
                    StatusMessage = "Грешка при обновяване на потребителското име.";
                    return RedirectToPage();
                }
            }

            string phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            if (Input.PhoneNumber != phoneNumber)
            {
                IdentityResult setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);

                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Грешка при обновяване на телефона.";
                    return RedirectToPage();
                }
            }

            IdentityResult updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                StatusMessage = "Грешка при обновяване на профила.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Профилът беше обновен успешно.";
            return RedirectToPage();
        }
    }
}