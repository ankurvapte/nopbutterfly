using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.DiscountRules.DiscountInCart.Models
{
    public record RequirementModel
    {
        public int DiscountId { get; set; }

        public int RequirementId { get; set; }

        [NopResourceDisplayName("Plugins.DiscountRules.DiscountInCart.Fields.Amount")]
        public string CartAmount { get; set; }
    }
}