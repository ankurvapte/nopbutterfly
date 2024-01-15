using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Factories;

namespace Nop.Plugin.DiscountRules.DiscountInCart
{
    public partial class DiscountIncartDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWebHelper _webHelper;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;

        public DiscountIncartDiscountRequirementRule(IActionContextAccessor actionContextAccessor,

            IDiscountService discountService,
            ILocalizationService localizationService,
            ISettingService settingService,
            IUrlHelperFactory urlHelperFactory,
            IWebHelper webHelper,
            IShoppingCartModelFactory shoppingCartModelFactory)
        {
            _actionContextAccessor = actionContextAccessor;
            _discountService = discountService;
            _localizationService = localizationService;
            _settingService = settingService;
            _urlHelperFactory = urlHelperFactory;
            _webHelper = webHelper;
            _shoppingCartModelFactory = shoppingCartModelFactory;
        }

        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public async Task<DiscountRequirementValidationResult> CheckRequirementAsync(DiscountRequirementValidationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            //invalid by default
            var result = new DiscountRequirementValidationResult();

            var cartTotal = await _shoppingCartModelFactory.PrepareMiniShoppingCartModelAsync();

            if (cartTotal == null || cartTotal.TotalProducts == 0)
            {
                return result;
            }

            var cartAmountRequirement = await _settingService.GetSettingByKeyAsync<string>($"DiscountRequirement.DiscountInCart-{request.DiscountRequirementId}");

            decimal CheckTotal1 = 0, CheckTotal2 = 0;

            if (cartAmountRequirement.Contains('-'))
            {
                string[] splt = cartAmountRequirement.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

                if (splt.Length > 1)
                {
                    decimal.TryParse(splt[0], NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out CheckTotal1);
                    decimal.TryParse(splt[1], NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out CheckTotal2);
                }
                else if (splt.Length == 1)
                {
                    decimal.TryParse(splt[0], NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out CheckTotal1);
                }
                else
                {
                    //unvalid
                    result.UserError = await _localizationService.GetResourceAsync("Plugins.DiscountRules.DiscountInCart.Fields.CartAmount.Required");
                    return result;
                }
            }
            else
            {
                decimal.TryParse(cartAmountRequirement, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out CheckTotal1);
            }

            if (CheckTotal1 == decimal.Zero)
            {
                //valid
                result.IsValid = true;
                return result;
            }            

            if (decimal.TryParse(cartTotal.SubTotal, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out decimal subTotal) && subTotal > 0)
            {
                if (subTotal >= CheckTotal1 && CheckTotal2 == 0)
                {
                    //valid
                    result.IsValid = true;
                }
                else if (CheckTotal2 > 0 && subTotal >= CheckTotal1 && subTotal <= CheckTotal2)
                {
                    //valid
                    result.IsValid = true;
                }
                else
                {
                    result.UserError = await _localizationService.GetResourceAsync("Plugins.DiscountRules.DiscountInCart.NotEnough");
                }
            }

            return result;
        }

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>
        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            return urlHelper.Action("Configure", "DiscountRulesDiscountInCart",
                new { discountId = discountId, discountRequirementId = discountRequirementId }, _webHelper.GetCurrentRequestProtocol());
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.DiscountRules.DiscountInCart.Fields.Amount"] = "Required cart amount or range of amounts. [$100 or $100-$200]",
                ["Plugins.DiscountRules.DiscountInCart.Fields.Amount.Hint"] = "Cart total $100 and more or range of $100-$200",
                ["Plugins.DiscountRules.DiscountInCart.NotEnough"] = "Sorry, this offer requires more cart amounts",
                ["Plugins.DiscountRules.DiscountInCart.Fields.CartAmount.Required"] = "Cart amount cannot be empty",
                ["Plugins.DiscountRules.DiscountInCart.Fields.DiscountId.Required"] = "Discount is required"
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //discount requirements
            var discountRequirements = (await _discountService.GetAllDiscountRequirementsAsync())
                .Where(discountRequirement => discountRequirement.DiscountRequirementRuleSystemName == DiscountRequirementDefaults.SYSTEM_NAME);
            foreach (var discountRequirement in discountRequirements)
            {
                await _discountService.DeleteDiscountRequirementAsync(discountRequirement, false);
            }

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.DiscountRules.DiscountInCart");

            await base.UninstallAsync();
        }
    }
}