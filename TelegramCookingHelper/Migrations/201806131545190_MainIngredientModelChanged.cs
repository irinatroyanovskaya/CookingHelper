namespace TelegramCookingHelper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MainIngredientModelChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MainIngredients", "ImageReference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MainIngredients", "ImageReference");
        }
    }
}
