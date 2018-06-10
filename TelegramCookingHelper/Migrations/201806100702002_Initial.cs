namespace TelegramCookingHelper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dishes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Recipe = c.String(),
                        MainIngredient_Id = c.Int(),
                        Meal_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MainIngredients", t => t.MainIngredient_Id)
                .ForeignKey("dbo.Meals", t => t.Meal_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.MainIngredient_Id)
                .Index(t => t.Meal_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.MainIngredients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WhereToBuy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Meals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Dishes", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Dishes", "Meal_Id", "dbo.Meals");
            DropForeignKey("dbo.Dishes", "MainIngredient_Id", "dbo.MainIngredients");
            DropIndex("dbo.Dishes", new[] { "User_Id" });
            DropIndex("dbo.Dishes", new[] { "Meal_Id" });
            DropIndex("dbo.Dishes", new[] { "MainIngredient_Id" });
            DropTable("dbo.Users");
            DropTable("dbo.Meals");
            DropTable("dbo.MainIngredients");
            DropTable("dbo.Dishes");
        }
    }
}
