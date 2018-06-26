using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System.Threading.Tasks;

namespace RestaurantDemo.Model
{
    [Serializable]
    public class OrderDetails
    {
        public MenuOption MenuItems { get; set; }

        //public DeliveryOptions DeliveryMode { get; set; }
        [Prompt("May I know your name?")]
        public string UserName { get; set; }

        [Numeric(1, 6)]
        [Prompt("how many {MenuItems.ItemName} do you want?")]
        public Int16 Totalno; 


        public static IForm<OrderDetails> BuildForm()
        {

            var menuItems = MenuDB.GetAllMenuOptions();

            var builder = new FormBuilder<OrderDetails>();

            builder
            .Message("Welcome to  Restaurant bot!")
            .Field(new FieldReflector<OrderDetails>(nameof(MenuItems))
            .SetType(null)
            .SetFieldDescription("Menu items")
            .SetDefine((state, field) =>
            {
                foreach (var item in menuItems)
                {
                    field
                    .AddDescription(item, new DescribeAttribute(title : item.ItemName, description : item.ItemName, subTitle : item.Description, image : item.ItemImage))
                    .AddTerms(item, item.ItemName);
                }

                return Task.FromResult(true);
            })
            .SetPrompt(new PromptAttribute("Select from the {&} \n {||} \n")
            {
                ChoiceStyle = ChoiceStyleOptions.Auto

            })
            .SetAllowsMultiple(false)
            )
            .AddRemainingFields()
            .Confirm(async (state) =>
            {
                return new PromptAttribute("Hi {UserName}.you choose {Totalno} of {MenuItems.ItemName}? {||}");
            })
            .OnCompletion(async (context, order) =>
            {
                await context.PostAsync("Thanks for your order!");
            });

            return builder.Build();
        }

    }
   

    [Serializable]
    public class MenuOption
    {
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string ItemImage { get; set; }
    }


}