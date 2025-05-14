using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommercePlateform.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaxType = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    DeliveryFeeType = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    FreeDeliveryAboveType = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    FreeDeliveryAbove = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AddressCart",
                columns: table => new
                {
                    AddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressCart", x => new { x.AddressesId, x.CartsId });
                });

            migrationBuilder.CreateTable(
                name: "AddressCartItem",
                columns: table => new
                {
                    AddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressCartItem", x => new { x.AddressesId, x.CartItemsId });
                });

            migrationBuilder.CreateTable(
                name: "AddressCoupen",
                columns: table => new
                {
                    AddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoupensId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressCoupen", x => new { x.AddressesId, x.CoupensId });
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Line1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Line2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Line3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    AddressType = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AddressSetting",
                columns: table => new
                {
                    AddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressSetting", x => new { x.AddressesId, x.SettingsId });
                    table.ForeignKey(
                        name: "FK_AddressSetting_Addresses_AddressesId",
                        column: x => x.AddressesId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressSetting_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressOrder",
                columns: table => new
                {
                    AddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressOrder", x => new { x.AddressesId, x.OrdersId });
                    table.ForeignKey(
                        name: "FK_AddressOrder_Addresses_AddressesId",
                        column: x => x.AddressesId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressOrderItem",
                columns: table => new
                {
                    AddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressOrderItem", x => new { x.AddressesId, x.OrderItemsId });
                    table.ForeignKey(
                        name: "FK_AddressOrderItem_Addresses_AddressesId",
                        column: x => x.AddressesId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressProduct",
                columns: table => new
                {
                    AddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressProduct", x => new { x.AddressesId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_AddressProduct_Addresses_AddressesId",
                        column: x => x.AddressesId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressProductVarient",
                columns: table => new
                {
                    AddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVarientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressProductVarient", x => new { x.AddressesId, x.ProductVarientsId });
                    table.ForeignKey(
                        name: "FK_AddressProductVarient_Addresses_AddressesId",
                        column: x => x.AddressesId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressReview",
                columns: table => new
                {
                    AddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressReview", x => new { x.AddressesId, x.ReviewsId });
                    table.ForeignKey(
                        name: "FK_AddressReview_Addresses_AddressesId",
                        column: x => x.AddressesId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressShippingAddress",
                columns: table => new
                {
                    AddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingAddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressShippingAddress", x => new { x.AddressesId, x.ShippingAddressesId });
                    table.ForeignKey(
                        name: "FK_AddressShippingAddress_Addresses_AddressesId",
                        column: x => x.AddressesId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartCity",
                columns: table => new
                {
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartCity", x => new { x.CartsId, x.CitiesId });
                });

            migrationBuilder.CreateTable(
                name: "CartCountry",
                columns: table => new
                {
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartCountry", x => new { x.CartsId, x.CountriesId });
                });

            migrationBuilder.CreateTable(
                name: "CartCoupen",
                columns: table => new
                {
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoupensId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartCoupen", x => new { x.CartsId, x.CoupensId });
                });

            migrationBuilder.CreateTable(
                name: "CartItemCity",
                columns: table => new
                {
                    CartItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemCity", x => new { x.CartItemsId, x.CitiesId });
                });

            migrationBuilder.CreateTable(
                name: "CartItemCountry",
                columns: table => new
                {
                    CartItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemCountry", x => new { x.CartItemsId, x.CountriesId });
                });

            migrationBuilder.CreateTable(
                name: "CartItemCoupen",
                columns: table => new
                {
                    CartItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoupensId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemCoupen", x => new { x.CartItemsId, x.CoupensId });
                });

            migrationBuilder.CreateTable(
                name: "CartItemOrder",
                columns: table => new
                {
                    CartItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemOrder", x => new { x.CartItemsId, x.OrdersId });
                });

            migrationBuilder.CreateTable(
                name: "CartItemOrderItem",
                columns: table => new
                {
                    CartItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemOrderItem", x => new { x.CartItemsId, x.OrderItemsId });
                });

            migrationBuilder.CreateTable(
                name: "CartItemReview",
                columns: table => new
                {
                    CartItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemReview", x => new { x.CartItemsId, x.ReviewsId });
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVarientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItemSetting",
                columns: table => new
                {
                    CartItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemSetting", x => new { x.CartItemsId, x.SettingsId });
                    table.ForeignKey(
                        name: "FK_CartItemSetting_CartItems_CartItemsId",
                        column: x => x.CartItemsId,
                        principalTable: "CartItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItemSetting_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItemShippingAddress",
                columns: table => new
                {
                    CartItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingAddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemShippingAddress", x => new { x.CartItemsId, x.ShippingAddressesId });
                    table.ForeignKey(
                        name: "FK_CartItemShippingAddress_CartItems_CartItemsId",
                        column: x => x.CartItemsId,
                        principalTable: "CartItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItemState",
                columns: table => new
                {
                    CartItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemState", x => new { x.CartItemsId, x.StatesId });
                    table.ForeignKey(
                        name: "FK_CartItemState_CartItems_CartItemsId",
                        column: x => x.CartItemsId,
                        principalTable: "CartItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItemUser",
                columns: table => new
                {
                    CartItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemUser", x => new { x.CartItemsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_CartItemUser_CartItems_CartItemsId",
                        column: x => x.CartItemsId,
                        principalTable: "CartItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartOrder",
                columns: table => new
                {
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartOrder", x => new { x.CartsId, x.OrdersId });
                });

            migrationBuilder.CreateTable(
                name: "CartOrderItem",
                columns: table => new
                {
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartOrderItem", x => new { x.CartsId, x.OrderItemsId });
                });

            migrationBuilder.CreateTable(
                name: "CartProduct",
                columns: table => new
                {
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartProduct", x => new { x.CartsId, x.ProductsId });
                });

            migrationBuilder.CreateTable(
                name: "CartProductVarient",
                columns: table => new
                {
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVarientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartProductVarient", x => new { x.CartsId, x.ProductVarientsId });
                });

            migrationBuilder.CreateTable(
                name: "CartReview",
                columns: table => new
                {
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartReview", x => new { x.CartsId, x.ReviewsId });
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalItems = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CartItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_CartItems_CartItemId",
                        column: x => x.CartItemId,
                        principalTable: "CartItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CartSetting",
                columns: table => new
                {
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartSetting", x => new { x.CartsId, x.SettingsId });
                    table.ForeignKey(
                        name: "FK_CartSetting_Carts_CartsId",
                        column: x => x.CartsId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartSetting_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartShippingAddress",
                columns: table => new
                {
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingAddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartShippingAddress", x => new { x.CartsId, x.ShippingAddressesId });
                    table.ForeignKey(
                        name: "FK_CartShippingAddress_Carts_CartsId",
                        column: x => x.CartsId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartState",
                columns: table => new
                {
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartState", x => new { x.CartsId, x.StatesId });
                    table.ForeignKey(
                        name: "FK_CartState_Carts_CartsId",
                        column: x => x.CartsId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShippingAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CitySetting",
                columns: table => new
                {
                    CitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitySetting", x => new { x.CitiesId, x.SettingsId });
                    table.ForeignKey(
                        name: "FK_CitySetting_Cities_CitiesId",
                        column: x => x.CitiesId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitySetting_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CityCountry",
                columns: table => new
                {
                    CitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityCountry", x => new { x.CitiesId, x.CountriesId });
                    table.ForeignKey(
                        name: "FK_CityCountry_Cities_CitiesId",
                        column: x => x.CitiesId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CityCoupen",
                columns: table => new
                {
                    CitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoupensId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityCoupen", x => new { x.CitiesId, x.CoupensId });
                    table.ForeignKey(
                        name: "FK_CityCoupen_Cities_CitiesId",
                        column: x => x.CitiesId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CityOrder",
                columns: table => new
                {
                    CitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityOrder", x => new { x.CitiesId, x.OrdersId });
                    table.ForeignKey(
                        name: "FK_CityOrder_Cities_CitiesId",
                        column: x => x.CitiesId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CityOrderItem",
                columns: table => new
                {
                    CitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityOrderItem", x => new { x.CitiesId, x.OrderItemsId });
                    table.ForeignKey(
                        name: "FK_CityOrderItem_Cities_CitiesId",
                        column: x => x.CitiesId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CityProduct",
                columns: table => new
                {
                    CitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityProduct", x => new { x.CitiesId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_CityProduct_Cities_CitiesId",
                        column: x => x.CitiesId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CityProductVarient",
                columns: table => new
                {
                    CitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVarientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityProductVarient", x => new { x.CitiesId, x.ProductVarientsId });
                    table.ForeignKey(
                        name: "FK_CityProductVarient_Cities_CitiesId",
                        column: x => x.CitiesId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CityReview",
                columns: table => new
                {
                    CitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityReview", x => new { x.CitiesId, x.ReviewsId });
                    table.ForeignKey(
                        name: "FK_CityReview_Cities_CitiesId",
                        column: x => x.CitiesId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CityUser",
                columns: table => new
                {
                    CitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityUser", x => new { x.CitiesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_CityUser_Cities_CitiesId",
                        column: x => x.CitiesId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShippingAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Countries_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CountrySetting",
                columns: table => new
                {
                    CountriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountrySetting", x => new { x.CountriesId, x.SettingsId });
                    table.ForeignKey(
                        name: "FK_CountrySetting_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountrySetting_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryCoupen",
                columns: table => new
                {
                    CountriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoupensId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryCoupen", x => new { x.CountriesId, x.CoupensId });
                    table.ForeignKey(
                        name: "FK_CountryCoupen_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryOrder",
                columns: table => new
                {
                    CountriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryOrder", x => new { x.CountriesId, x.OrdersId });
                    table.ForeignKey(
                        name: "FK_CountryOrder_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryOrderItem",
                columns: table => new
                {
                    CountriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryOrderItem", x => new { x.CountriesId, x.OrderItemsId });
                    table.ForeignKey(
                        name: "FK_CountryOrderItem_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryProduct",
                columns: table => new
                {
                    CountriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryProduct", x => new { x.CountriesId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_CountryProduct_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryProductVarient",
                columns: table => new
                {
                    CountriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVarientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryProductVarient", x => new { x.CountriesId, x.ProductVarientsId });
                    table.ForeignKey(
                        name: "FK_CountryProductVarient_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryReview",
                columns: table => new
                {
                    CountriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryReview", x => new { x.CountriesId, x.ReviewsId });
                    table.ForeignKey(
                        name: "FK_CountryReview_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryUser",
                columns: table => new
                {
                    CountriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryUser", x => new { x.CountriesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_CountryUser_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoupenOrderItem",
                columns: table => new
                {
                    CoupensId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoupenOrderItem", x => new { x.CoupensId, x.OrderItemsId });
                });

            migrationBuilder.CreateTable(
                name: "CoupenReview",
                columns: table => new
                {
                    CoupensId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoupenReview", x => new { x.CoupensId, x.ReviewsId });
                });

            migrationBuilder.CreateTable(
                name: "Coupens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVarientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiscountType = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    MinimumValue = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    MaximumValue = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    MinimumQuantity = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    MaximumQuantity = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    ValidityPeriod = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    ValidityStartDate = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: false),
                    ValidityEndDate = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: false),
                    TotalUseCount = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    RemainingUseCount = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    TotalLimitedUseCount = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsExpired = table.Column<bool>(type: "bit", nullable: false),
                    IsOneTimeUse = table.Column<bool>(type: "bit", nullable: false),
                    IsLimitedUse = table.Column<bool>(type: "bit", nullable: false),
                    IsLimitedUsePerUser = table.Column<bool>(type: "bit", nullable: false),
                    IsLimitedUsePerProduct = table.Column<bool>(type: "bit", nullable: false),
                    IsLimitedUsePerProductVarient = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoupenSetting",
                columns: table => new
                {
                    CoupensId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoupenSetting", x => new { x.CoupensId, x.SettingsId });
                    table.ForeignKey(
                        name: "FK_CoupenSetting_Coupens_CoupensId",
                        column: x => x.CoupensId,
                        principalTable: "Coupens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoupenSetting_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoupenShippingAddress",
                columns: table => new
                {
                    CoupensId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingAddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoupenShippingAddress", x => new { x.CoupensId, x.ShippingAddressesId });
                    table.ForeignKey(
                        name: "FK_CoupenShippingAddress_Coupens_CoupensId",
                        column: x => x.CoupensId,
                        principalTable: "Coupens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoupenState",
                columns: table => new
                {
                    CoupensId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoupenState", x => new { x.CoupensId, x.StatesId });
                    table.ForeignKey(
                        name: "FK_CoupenState_Coupens_CoupensId",
                        column: x => x.CoupensId,
                        principalTable: "Coupens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoupenUser",
                columns: table => new
                {
                    CoupensId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoupenUser", x => new { x.CoupensId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_CoupenUser_Coupens_CoupensId",
                        column: x => x.CoupensId,
                        principalTable: "Coupens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemReview",
                columns: table => new
                {
                    OrderItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemReview", x => new { x.OrderItemsId, x.ReviewsId });
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVarientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemSetting",
                columns: table => new
                {
                    OrderItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemSetting", x => new { x.OrderItemsId, x.SettingsId });
                    table.ForeignKey(
                        name: "FK_OrderItemSetting_OrderItems_OrderItemsId",
                        column: x => x.OrderItemsId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemSetting_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemShippingAddress",
                columns: table => new
                {
                    OrderItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingAddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemShippingAddress", x => new { x.OrderItemsId, x.ShippingAddressesId });
                    table.ForeignKey(
                        name: "FK_OrderItemShippingAddress_OrderItems_OrderItemsId",
                        column: x => x.OrderItemsId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemState",
                columns: table => new
                {
                    OrderItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemState", x => new { x.OrderItemsId, x.StatesId });
                    table.ForeignKey(
                        name: "FK_OrderItemState_OrderItems_OrderItemsId",
                        column: x => x.OrderItemsId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemUser",
                columns: table => new
                {
                    OrderItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemUser", x => new { x.OrderItemsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_OrderItemUser_OrderItems_OrderItemsId",
                        column: x => x.OrderItemsId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderProduct",
                columns: table => new
                {
                    OrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProduct", x => new { x.OrdersId, x.ProductsId });
                });

            migrationBuilder.CreateTable(
                name: "OrderProductVarient",
                columns: table => new
                {
                    OrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVarientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProductVarient", x => new { x.OrdersId, x.ProductVarientsId });
                });

            migrationBuilder.CreateTable(
                name: "OrderReview",
                columns: table => new
                {
                    OrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderReview", x => new { x.OrdersId, x.ReviewsId });
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoupenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalItems = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    ShippingAmount = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    OrderStatus = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Coupens_CoupenId",
                        column: x => x.CoupenId,
                        principalTable: "Coupens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderSetting",
                columns: table => new
                {
                    OrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSetting", x => new { x.OrdersId, x.SettingsId });
                    table.ForeignKey(
                        name: "FK_OrderSetting_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderSetting_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderState",
                columns: table => new
                {
                    OrdersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderState", x => new { x.OrdersId, x.StatesId });
                    table.ForeignKey(
                        name: "FK_OrderState_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalStockQuantity = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CartItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CoupenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductVarientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_CartItems_CartItemId",
                        column: x => x.CartItemId,
                        principalTable: "CartItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_Coupens_CoupenId",
                        column: x => x.CoupenId,
                        principalTable: "Coupens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductSetting",
                columns: table => new
                {
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSetting", x => new { x.ProductsId, x.SettingsId });
                    table.ForeignKey(
                        name: "FK_ProductSetting_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSetting_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVarients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", maxLength: 20, precision: 18, scale: 2, nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StockQuantity = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CartItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CoupenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVarients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVarients_CartItems_CartItemId",
                        column: x => x.CartItemId,
                        principalTable: "CartItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductVarients_Coupens_CoupenId",
                        column: x => x.CoupenId,
                        principalTable: "Coupens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductVarients_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductVarients_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVarientSetting",
                columns: table => new
                {
                    ProductVarientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVarientSetting", x => new { x.ProductVarientsId, x.SettingsId });
                    table.ForeignKey(
                        name: "FK_ProductVarientSetting_ProductVarients_ProductVarientsId",
                        column: x => x.ProductVarientsId,
                        principalTable: "ProductVarients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVarientSetting_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductShippingAddress",
                columns: table => new
                {
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingAddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductShippingAddress", x => new { x.ProductsId, x.ShippingAddressesId });
                    table.ForeignKey(
                        name: "FK_ProductShippingAddress_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductState",
                columns: table => new
                {
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductState", x => new { x.ProductsId, x.StatesId });
                    table.ForeignKey(
                        name: "FK_ProductState_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductUser",
                columns: table => new
                {
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUser", x => new { x.ProductsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ProductUser_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVarientReview",
                columns: table => new
                {
                    ProductVarientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVarientReview", x => new { x.ProductVarientsId, x.ReviewsId });
                    table.ForeignKey(
                        name: "FK_ProductVarientReview_ProductVarients_ProductVarientsId",
                        column: x => x.ProductVarientsId,
                        principalTable: "ProductVarients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVarientShippingAddress",
                columns: table => new
                {
                    ProductVarientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingAddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVarientShippingAddress", x => new { x.ProductVarientsId, x.ShippingAddressesId });
                    table.ForeignKey(
                        name: "FK_ProductVarientShippingAddress_ProductVarients_ProductVarientsId",
                        column: x => x.ProductVarientsId,
                        principalTable: "ProductVarients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVarientState",
                columns: table => new
                {
                    ProductVarientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVarientState", x => new { x.ProductVarientsId, x.StatesId });
                    table.ForeignKey(
                        name: "FK_ProductVarientState_ProductVarients_ProductVarientsId",
                        column: x => x.ProductVarientsId,
                        principalTable: "ProductVarients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVarientUser",
                columns: table => new
                {
                    ProductVarientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVarientUser", x => new { x.ProductVarientsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ProductVarientUser_ProductVarients_ProductVarientsId",
                        column: x => x.ProductVarientsId,
                        principalTable: "ProductVarients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", maxLength: 1, nullable: false),
                    ReviewText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    IsReported = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewSetting",
                columns: table => new
                {
                    ReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewSetting", x => new { x.ReviewsId, x.SettingsId });
                    table.ForeignKey(
                        name: "FK_ReviewSetting_Reviews_ReviewsId",
                        column: x => x.ReviewsId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewSetting_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewShippingAddress",
                columns: table => new
                {
                    ReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingAddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewShippingAddress", x => new { x.ReviewsId, x.ShippingAddressesId });
                    table.ForeignKey(
                        name: "FK_ReviewShippingAddress_Reviews_ReviewsId",
                        column: x => x.ReviewsId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewState",
                columns: table => new
                {
                    ReviewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewState", x => new { x.ReviewsId, x.StatesId });
                    table.ForeignKey(
                        name: "FK_ReviewState_Reviews_ReviewsId",
                        column: x => x.ReviewsId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SettingShippingAddress",
                columns: table => new
                {
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShippingAddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingShippingAddress", x => new { x.SettingsId, x.ShippingAddressesId });
                    table.ForeignKey(
                        name: "FK_SettingShippingAddress_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SettingState",
                columns: table => new
                {
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingState", x => new { x.SettingsId, x.StatesId });
                    table.ForeignKey(
                        name: "FK_SettingState_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SettingUser",
                columns: table => new
                {
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingUser", x => new { x.SettingsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_SettingUser_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Line1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Line2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Line3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    AddressType = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingAddresses_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingAddresses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingAddresses_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShippingAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                    table.ForeignKey(
                        name: "FK_States_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_States_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_States_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_States_ShippingAddresses_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "ShippingAddresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Avatar = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfirmPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShippingAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_ShippingAddresses_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "ShippingAddresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StateUser",
                columns: table => new
                {
                    StatesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateUser", x => new { x.StatesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_StateUser_States_StatesId",
                        column: x => x.StatesId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StateUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AddressId", "Avatar", "Bio", "CartId", "ConfirmPassword", "CreatedBy", "CreatedOn", "DateOfBirth", "Email", "FirstName", "Gender", "IsActive", "IsDeleted", "LastName", "ModifiedBy", "ModifiedOn", "OrderId", "Password", "PhoneNumber", "ReviewId", "Role", "ShippingAddressId" },
                values: new object[] { new Guid("e65a3a8a-2407-4965-9b71-b9a1d8e2c34f"), null, null, "System Administrator", null, "Admin@123", "System", new DateTime(2025, 5, 2, 14, 56, 57, 234, DateTimeKind.Local).AddTicks(4047), new DateOnly(1990, 1, 1), "admin@admin.com", "Admin", 0, true, false, "User", "System", new DateTime(2025, 5, 2, 14, 56, 57, 234, DateTimeKind.Local).AddTicks(4297), null, "Admin@123", "1234567890", null, 0, null });

            migrationBuilder.CreateIndex(
                name: "IX_AddressCart_CartsId",
                table: "AddressCart",
                column: "CartsId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressCartItem_CartItemsId",
                table: "AddressCartItem",
                column: "CartItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressCoupen_CoupensId",
                table: "AddressCoupen",
                column: "CoupensId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CityId",
                table: "Addresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_StateId",
                table: "Addresses",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressOrder_OrdersId",
                table: "AddressOrder",
                column: "OrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressOrderItem_OrderItemsId",
                table: "AddressOrderItem",
                column: "OrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressProduct_ProductsId",
                table: "AddressProduct",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressProductVarient_ProductVarientsId",
                table: "AddressProductVarient",
                column: "ProductVarientsId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressReview_ReviewsId",
                table: "AddressReview",
                column: "ReviewsId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressSetting_SettingsId",
                table: "AddressSetting",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressShippingAddress_ShippingAddressesId",
                table: "AddressShippingAddress",
                column: "ShippingAddressesId");

            migrationBuilder.CreateIndex(
                name: "IX_CartCity_CitiesId",
                table: "CartCity",
                column: "CitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_CartCountry_CountriesId",
                table: "CartCountry",
                column: "CountriesId");

            migrationBuilder.CreateIndex(
                name: "IX_CartCoupen_CoupensId",
                table: "CartCoupen",
                column: "CoupensId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemCity_CitiesId",
                table: "CartItemCity",
                column: "CitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemCountry_CountriesId",
                table: "CartItemCountry",
                column: "CountriesId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemCoupen_CoupensId",
                table: "CartItemCoupen",
                column: "CoupensId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemOrder_OrdersId",
                table: "CartItemOrder",
                column: "OrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemOrderItem_OrderItemsId",
                table: "CartItemOrderItem",
                column: "OrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemReview_ReviewsId",
                table: "CartItemReview",
                column: "ReviewsId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductVarientId",
                table: "CartItems",
                column: "ProductVarientId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemSetting_SettingsId",
                table: "CartItemSetting",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemShippingAddress_ShippingAddressesId",
                table: "CartItemShippingAddress",
                column: "ShippingAddressesId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemState_StatesId",
                table: "CartItemState",
                column: "StatesId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemUser_UsersId",
                table: "CartItemUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_CartOrder_OrdersId",
                table: "CartOrder",
                column: "OrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_CartOrderItem_OrderItemsId",
                table: "CartOrderItem",
                column: "OrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_CartProduct_ProductsId",
                table: "CartProduct",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_CartProductVarient_ProductVarientsId",
                table: "CartProductVarient",
                column: "ProductVarientsId");

            migrationBuilder.CreateIndex(
                name: "IX_CartReview_ReviewsId",
                table: "CartReview",
                column: "ReviewsId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CartItemId",
                table: "Carts",
                column: "CartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartSetting_SettingsId",
                table: "CartSetting",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CartShippingAddress_ShippingAddressesId",
                table: "CartShippingAddress",
                column: "ShippingAddressesId");

            migrationBuilder.CreateIndex(
                name: "IX_CartState_StatesId",
                table: "CartState",
                column: "StatesId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_AddressId",
                table: "Cities",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name_StateId",
                table: "Cities",
                columns: new[] { "Name", "StateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ShippingAddressId",
                table: "Cities",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_StateId",
                table: "Cities",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_CityCountry_CountriesId",
                table: "CityCountry",
                column: "CountriesId");

            migrationBuilder.CreateIndex(
                name: "IX_CityCoupen_CoupensId",
                table: "CityCoupen",
                column: "CoupensId");

            migrationBuilder.CreateIndex(
                name: "IX_CityOrder_OrdersId",
                table: "CityOrder",
                column: "OrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_CityOrderItem_OrderItemsId",
                table: "CityOrderItem",
                column: "OrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_CityProduct_ProductsId",
                table: "CityProduct",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_CityProductVarient_ProductVarientsId",
                table: "CityProductVarient",
                column: "ProductVarientsId");

            migrationBuilder.CreateIndex(
                name: "IX_CityReview_ReviewsId",
                table: "CityReview",
                column: "ReviewsId");

            migrationBuilder.CreateIndex(
                name: "IX_CitySetting_SettingsId",
                table: "CitySetting",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CityUser_UsersId",
                table: "CityUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_AddressId",
                table: "Countries",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name_Code",
                table: "Countries",
                columns: new[] { "Name", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_ShippingAddressId",
                table: "Countries",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_StateId",
                table: "Countries",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryCoupen_CoupensId",
                table: "CountryCoupen",
                column: "CoupensId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryOrder_OrdersId",
                table: "CountryOrder",
                column: "OrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryOrderItem_OrderItemsId",
                table: "CountryOrderItem",
                column: "OrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryProduct_ProductsId",
                table: "CountryProduct",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryProductVarient_ProductVarientsId",
                table: "CountryProductVarient",
                column: "ProductVarientsId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryReview_ReviewsId",
                table: "CountryReview",
                column: "ReviewsId");

            migrationBuilder.CreateIndex(
                name: "IX_CountrySetting_SettingsId",
                table: "CountrySetting",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryUser_UsersId",
                table: "CountryUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_CoupenOrderItem_OrderItemsId",
                table: "CoupenOrderItem",
                column: "OrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_CoupenReview_ReviewsId",
                table: "CoupenReview",
                column: "ReviewsId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupens_Code",
                table: "Coupens",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupens_OrderId",
                table: "Coupens",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupens_ProductId",
                table: "Coupens",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupens_ProductVarientId",
                table: "Coupens",
                column: "ProductVarientId");

            migrationBuilder.CreateIndex(
                name: "IX_CoupenSetting_SettingsId",
                table: "CoupenSetting",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CoupenShippingAddress_ShippingAddressesId",
                table: "CoupenShippingAddress",
                column: "ShippingAddressesId");

            migrationBuilder.CreateIndex(
                name: "IX_CoupenState_StatesId",
                table: "CoupenState",
                column: "StatesId");

            migrationBuilder.CreateIndex(
                name: "IX_CoupenUser_UsersId",
                table: "CoupenUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemReview_ReviewsId",
                table: "OrderItemReview",
                column: "ReviewsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductVarientId",
                table: "OrderItems",
                column: "ProductVarientId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemSetting_SettingsId",
                table: "OrderItemSetting",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemShippingAddress_ShippingAddressesId",
                table: "OrderItemShippingAddress",
                column: "ShippingAddressesId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemState_StatesId",
                table: "OrderItemState",
                column: "StatesId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemUser_UsersId",
                table: "OrderItemUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_ProductsId",
                table: "OrderProduct",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductVarient_ProductVarientsId",
                table: "OrderProductVarient",
                column: "ProductVarientsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderReview_ReviewsId",
                table: "OrderReview",
                column: "ReviewsId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CoupenId",
                table: "Orders",
                column: "CoupenId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderItemId",
                table: "Orders",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingAddressId",
                table: "Orders",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSetting_SettingsId",
                table: "OrderSetting",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderState_StatesId",
                table: "OrderState",
                column: "StatesId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CartItemId",
                table: "Products",
                column: "CartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CoupenId",
                table: "Products",
                column: "CoupenId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrderItemId",
                table: "Products",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductVarientId",
                table: "Products",
                column: "ProductVarientId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ReviewId",
                table: "Products",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true,
                filter: "[SKU] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSetting_SettingsId",
                table: "ProductSetting",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductShippingAddress_ShippingAddressesId",
                table: "ProductShippingAddress",
                column: "ShippingAddressesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductState_StatesId",
                table: "ProductState",
                column: "StatesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUser_UsersId",
                table: "ProductUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVarientReview_ReviewsId",
                table: "ProductVarientReview",
                column: "ReviewsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVarients_CartItemId",
                table: "ProductVarients",
                column: "CartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVarients_CoupenId",
                table: "ProductVarients",
                column: "CoupenId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVarients_OrderItemId",
                table: "ProductVarients",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVarients_ProductId",
                table: "ProductVarients",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVarients_SKU",
                table: "ProductVarients",
                column: "SKU",
                unique: true,
                filter: "[SKU] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVarientSetting_SettingsId",
                table: "ProductVarientSetting",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVarientShippingAddress_ShippingAddressesId",
                table: "ProductVarientShippingAddress",
                column: "ShippingAddressesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVarientState_StatesId",
                table: "ProductVarientState",
                column: "StatesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVarientUser_UsersId",
                table: "ProductVarientUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewSetting_SettingsId",
                table: "ReviewSetting",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewShippingAddress_ShippingAddressesId",
                table: "ReviewShippingAddress",
                column: "ShippingAddressesId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewState_StatesId",
                table: "ReviewState",
                column: "StatesId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingShippingAddress_ShippingAddressesId",
                table: "SettingShippingAddress",
                column: "ShippingAddressesId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingState_StatesId",
                table: "SettingState",
                column: "StatesId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingUser_UsersId",
                table: "SettingUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddresses_CityId",
                table: "ShippingAddresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddresses_CountryId",
                table: "ShippingAddresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddresses_OrderId",
                table: "ShippingAddresses",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddresses_StateId",
                table: "ShippingAddresses",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddresses_UserId",
                table: "ShippingAddresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_States_AddressId",
                table: "States",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_States_CityId",
                table: "States",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_States_Code_CountryId",
                table: "States",
                columns: new[] { "Code", "CountryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_Code_Name_CountryId",
                table: "States",
                columns: new[] { "Code", "Name", "CountryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_CountryId",
                table: "States",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_States_Name_CountryId",
                table: "States",
                columns: new[] { "Name", "CountryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_ShippingAddressId",
                table: "States",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_StateUser_UsersId",
                table: "StateUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressId",
                table: "Users",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CartId",
                table: "Users",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_OrderId",
                table: "Users",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ReviewId",
                table: "Users",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ShippingAddressId",
                table: "Users",
                column: "ShippingAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressCart_Addresses_AddressesId",
                table: "AddressCart",
                column: "AddressesId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressCart_Carts_CartsId",
                table: "AddressCart",
                column: "CartsId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressCartItem_Addresses_AddressesId",
                table: "AddressCartItem",
                column: "AddressesId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressCartItem_CartItems_CartItemsId",
                table: "AddressCartItem",
                column: "CartItemsId",
                principalTable: "CartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressCoupen_Addresses_AddressesId",
                table: "AddressCoupen",
                column: "AddressesId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressCoupen_Coupens_CoupensId",
                table: "AddressCoupen",
                column: "CoupensId",
                principalTable: "Coupens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Cities_CityId",
                table: "Addresses",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_States_StateId",
                table: "Addresses",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_UserId",
                table: "Addresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressOrder_Orders_OrdersId",
                table: "AddressOrder",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressOrderItem_OrderItems_OrderItemsId",
                table: "AddressOrderItem",
                column: "OrderItemsId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressProduct_Products_ProductsId",
                table: "AddressProduct",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressProductVarient_ProductVarients_ProductVarientsId",
                table: "AddressProductVarient",
                column: "ProductVarientsId",
                principalTable: "ProductVarients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressReview_Reviews_ReviewsId",
                table: "AddressReview",
                column: "ReviewsId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AddressShippingAddress_ShippingAddresses_ShippingAddressesId",
                table: "AddressShippingAddress",
                column: "ShippingAddressesId",
                principalTable: "ShippingAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartCity_Carts_CartsId",
                table: "CartCity",
                column: "CartsId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartCity_Cities_CitiesId",
                table: "CartCity",
                column: "CitiesId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartCountry_Carts_CartsId",
                table: "CartCountry",
                column: "CartsId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartCountry_Countries_CountriesId",
                table: "CartCountry",
                column: "CountriesId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartCoupen_Carts_CartsId",
                table: "CartCoupen",
                column: "CartsId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartCoupen_Coupens_CoupensId",
                table: "CartCoupen",
                column: "CoupensId",
                principalTable: "Coupens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemCity_CartItems_CartItemsId",
                table: "CartItemCity",
                column: "CartItemsId",
                principalTable: "CartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemCity_Cities_CitiesId",
                table: "CartItemCity",
                column: "CitiesId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemCountry_CartItems_CartItemsId",
                table: "CartItemCountry",
                column: "CartItemsId",
                principalTable: "CartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemCountry_Countries_CountriesId",
                table: "CartItemCountry",
                column: "CountriesId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemCoupen_CartItems_CartItemsId",
                table: "CartItemCoupen",
                column: "CartItemsId",
                principalTable: "CartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemCoupen_Coupens_CoupensId",
                table: "CartItemCoupen",
                column: "CoupensId",
                principalTable: "Coupens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemOrder_CartItems_CartItemsId",
                table: "CartItemOrder",
                column: "CartItemsId",
                principalTable: "CartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemOrder_Orders_OrdersId",
                table: "CartItemOrder",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemOrderItem_CartItems_CartItemsId",
                table: "CartItemOrderItem",
                column: "CartItemsId",
                principalTable: "CartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemOrderItem_OrderItems_OrderItemsId",
                table: "CartItemOrderItem",
                column: "OrderItemsId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemReview_CartItems_CartItemsId",
                table: "CartItemReview",
                column: "CartItemsId",
                principalTable: "CartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemReview_Reviews_ReviewsId",
                table: "CartItemReview",
                column: "ReviewsId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ProductVarients_ProductVarientId",
                table: "CartItems",
                column: "ProductVarientId",
                principalTable: "ProductVarients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemShippingAddress_ShippingAddresses_ShippingAddressesId",
                table: "CartItemShippingAddress",
                column: "ShippingAddressesId",
                principalTable: "ShippingAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemState_States_StatesId",
                table: "CartItemState",
                column: "StatesId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemUser_Users_UsersId",
                table: "CartItemUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartOrder_Carts_CartsId",
                table: "CartOrder",
                column: "CartsId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartOrder_Orders_OrdersId",
                table: "CartOrder",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartOrderItem_Carts_CartsId",
                table: "CartOrderItem",
                column: "CartsId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartOrderItem_OrderItems_OrderItemsId",
                table: "CartOrderItem",
                column: "OrderItemsId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartProduct_Carts_CartsId",
                table: "CartProduct",
                column: "CartsId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartProduct_Products_ProductsId",
                table: "CartProduct",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartProductVarient_Carts_CartsId",
                table: "CartProductVarient",
                column: "CartsId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartProductVarient_ProductVarients_ProductVarientsId",
                table: "CartProductVarient",
                column: "ProductVarientsId",
                principalTable: "ProductVarients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartReview_Carts_CartsId",
                table: "CartReview",
                column: "CartsId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartReview_Reviews_ReviewsId",
                table: "CartReview",
                column: "ReviewsId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartShippingAddress_ShippingAddresses_ShippingAddressesId",
                table: "CartShippingAddress",
                column: "ShippingAddressesId",
                principalTable: "ShippingAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartState_States_StatesId",
                table: "CartState",
                column: "StatesId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_ShippingAddresses_ShippingAddressId",
                table: "Cities",
                column: "ShippingAddressId",
                principalTable: "ShippingAddresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_States_StateId",
                table: "Cities",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CityCountry_Countries_CountriesId",
                table: "CityCountry",
                column: "CountriesId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CityCoupen_Coupens_CoupensId",
                table: "CityCoupen",
                column: "CoupensId",
                principalTable: "Coupens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CityOrder_Orders_OrdersId",
                table: "CityOrder",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CityOrderItem_OrderItems_OrderItemsId",
                table: "CityOrderItem",
                column: "OrderItemsId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CityProduct_Products_ProductsId",
                table: "CityProduct",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CityProductVarient_ProductVarients_ProductVarientsId",
                table: "CityProductVarient",
                column: "ProductVarientsId",
                principalTable: "ProductVarients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CityReview_Reviews_ReviewsId",
                table: "CityReview",
                column: "ReviewsId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CityUser_Users_UsersId",
                table: "CityUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_ShippingAddresses_ShippingAddressId",
                table: "Countries",
                column: "ShippingAddressId",
                principalTable: "ShippingAddresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_States_StateId",
                table: "Countries",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CountryCoupen_Coupens_CoupensId",
                table: "CountryCoupen",
                column: "CoupensId",
                principalTable: "Coupens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CountryOrder_Orders_OrdersId",
                table: "CountryOrder",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CountryOrderItem_OrderItems_OrderItemsId",
                table: "CountryOrderItem",
                column: "OrderItemsId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CountryProduct_Products_ProductsId",
                table: "CountryProduct",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CountryProductVarient_ProductVarients_ProductVarientsId",
                table: "CountryProductVarient",
                column: "ProductVarientsId",
                principalTable: "ProductVarients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CountryReview_Reviews_ReviewsId",
                table: "CountryReview",
                column: "ReviewsId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CountryUser_Users_UsersId",
                table: "CountryUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoupenOrderItem_Coupens_CoupensId",
                table: "CoupenOrderItem",
                column: "CoupensId",
                principalTable: "Coupens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoupenOrderItem_OrderItems_OrderItemsId",
                table: "CoupenOrderItem",
                column: "OrderItemsId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoupenReview_Coupens_CoupensId",
                table: "CoupenReview",
                column: "CoupensId",
                principalTable: "Coupens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoupenReview_Reviews_ReviewsId",
                table: "CoupenReview",
                column: "ReviewsId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Coupens_Orders_OrderId",
                table: "Coupens",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupens_ProductVarients_ProductVarientId",
                table: "Coupens",
                column: "ProductVarientId",
                principalTable: "ProductVarients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Coupens_Products_ProductId",
                table: "Coupens",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CoupenShippingAddress_ShippingAddresses_ShippingAddressesId",
                table: "CoupenShippingAddress",
                column: "ShippingAddressesId",
                principalTable: "ShippingAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoupenState_States_StatesId",
                table: "CoupenState",
                column: "StatesId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoupenUser_Users_UsersId",
                table: "CoupenUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemReview_OrderItems_OrderItemsId",
                table: "OrderItemReview",
                column: "OrderItemsId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemReview_Reviews_ReviewsId",
                table: "OrderItemReview",
                column: "ReviewsId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductVarients_ProductVarientId",
                table: "OrderItems",
                column: "ProductVarientId",
                principalTable: "ProductVarients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemShippingAddress_ShippingAddresses_ShippingAddressesId",
                table: "OrderItemShippingAddress",
                column: "ShippingAddressesId",
                principalTable: "ShippingAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemState_States_StatesId",
                table: "OrderItemState",
                column: "StatesId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemUser_Users_UsersId",
                table: "OrderItemUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProduct_Orders_OrdersId",
                table: "OrderProduct",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProduct_Products_ProductsId",
                table: "OrderProduct",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductVarient_Orders_OrdersId",
                table: "OrderProductVarient",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductVarient_ProductVarients_ProductVarientsId",
                table: "OrderProductVarient",
                column: "ProductVarientsId",
                principalTable: "ProductVarients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderReview_Orders_OrdersId",
                table: "OrderReview",
                column: "OrdersId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderReview_Reviews_ReviewsId",
                table: "OrderReview",
                column: "ReviewsId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ShippingAddresses_ShippingAddressId",
                table: "Orders",
                column: "ShippingAddressId",
                principalTable: "ShippingAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderState_States_StatesId",
                table: "OrderState",
                column: "StatesId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductVarients_ProductVarientId",
                table: "Products",
                column: "ProductVarientId",
                principalTable: "ProductVarients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Reviews_ReviewId",
                table: "Products",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductShippingAddress_ShippingAddresses_ShippingAddressesId",
                table: "ProductShippingAddress",
                column: "ShippingAddressesId",
                principalTable: "ShippingAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductState_States_StatesId",
                table: "ProductState",
                column: "StatesId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductUser_Users_UsersId",
                table: "ProductUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVarientReview_Reviews_ReviewsId",
                table: "ProductVarientReview",
                column: "ReviewsId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVarientShippingAddress_ShippingAddresses_ShippingAddressesId",
                table: "ProductVarientShippingAddress",
                column: "ShippingAddressesId",
                principalTable: "ShippingAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVarientState_States_StatesId",
                table: "ProductVarientState",
                column: "StatesId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVarientUser_Users_UsersId",
                table: "ProductVarientUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewShippingAddress_ShippingAddresses_ShippingAddressesId",
                table: "ReviewShippingAddress",
                column: "ShippingAddressesId",
                principalTable: "ShippingAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewState_States_StatesId",
                table: "ReviewState",
                column: "StatesId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SettingShippingAddress_ShippingAddresses_ShippingAddressesId",
                table: "SettingShippingAddress",
                column: "ShippingAddressesId",
                principalTable: "ShippingAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SettingState_States_StatesId",
                table: "SettingState",
                column: "StatesId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SettingUser_Users_UsersId",
                table: "SettingUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingAddresses_States_StateId",
                table: "ShippingAddresses",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingAddresses_Users_UserId",
                table: "ShippingAddresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Addresses_AddressId",
                table: "Cities");

            migrationBuilder.DropForeignKey(
                name: "FK_Countries_Addresses_AddressId",
                table: "Countries");

            migrationBuilder.DropForeignKey(
                name: "FK_States_Addresses_AddressId",
                table: "States");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Addresses_AddressId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Carts_CartId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_CartItems_CartItemId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVarients_CartItems_CartItemId",
                table: "ProductVarients");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Coupens_CoupenId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Coupens_CoupenId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVarients_Coupens_CoupenId",
                table: "ProductVarients");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingAddresses_Cities_CityId",
                table: "ShippingAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_States_Cities_CityId",
                table: "States");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingAddresses_Countries_CountryId",
                table: "ShippingAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_States_Countries_CountryId",
                table: "States");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingAddresses_States_StateId",
                table: "ShippingAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingAddresses_Users_UserId",
                table: "ShippingAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingAddresses_Orders_OrderId",
                table: "ShippingAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_OrderItems_OrderItemId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVarients_OrderItems_OrderItemId",
                table: "ProductVarients");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVarients_Products_ProductId",
                table: "ProductVarients");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Products_ProductId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "AddressCart");

            migrationBuilder.DropTable(
                name: "AddressCartItem");

            migrationBuilder.DropTable(
                name: "AddressCoupen");

            migrationBuilder.DropTable(
                name: "AddressOrder");

            migrationBuilder.DropTable(
                name: "AddressOrderItem");

            migrationBuilder.DropTable(
                name: "AddressProduct");

            migrationBuilder.DropTable(
                name: "AddressProductVarient");

            migrationBuilder.DropTable(
                name: "AddressReview");

            migrationBuilder.DropTable(
                name: "AddressSetting");

            migrationBuilder.DropTable(
                name: "AddressShippingAddress");

            migrationBuilder.DropTable(
                name: "CartCity");

            migrationBuilder.DropTable(
                name: "CartCountry");

            migrationBuilder.DropTable(
                name: "CartCoupen");

            migrationBuilder.DropTable(
                name: "CartItemCity");

            migrationBuilder.DropTable(
                name: "CartItemCountry");

            migrationBuilder.DropTable(
                name: "CartItemCoupen");

            migrationBuilder.DropTable(
                name: "CartItemOrder");

            migrationBuilder.DropTable(
                name: "CartItemOrderItem");

            migrationBuilder.DropTable(
                name: "CartItemReview");

            migrationBuilder.DropTable(
                name: "CartItemSetting");

            migrationBuilder.DropTable(
                name: "CartItemShippingAddress");

            migrationBuilder.DropTable(
                name: "CartItemState");

            migrationBuilder.DropTable(
                name: "CartItemUser");

            migrationBuilder.DropTable(
                name: "CartOrder");

            migrationBuilder.DropTable(
                name: "CartOrderItem");

            migrationBuilder.DropTable(
                name: "CartProduct");

            migrationBuilder.DropTable(
                name: "CartProductVarient");

            migrationBuilder.DropTable(
                name: "CartReview");

            migrationBuilder.DropTable(
                name: "CartSetting");

            migrationBuilder.DropTable(
                name: "CartShippingAddress");

            migrationBuilder.DropTable(
                name: "CartState");

            migrationBuilder.DropTable(
                name: "CityCountry");

            migrationBuilder.DropTable(
                name: "CityCoupen");

            migrationBuilder.DropTable(
                name: "CityOrder");

            migrationBuilder.DropTable(
                name: "CityOrderItem");

            migrationBuilder.DropTable(
                name: "CityProduct");

            migrationBuilder.DropTable(
                name: "CityProductVarient");

            migrationBuilder.DropTable(
                name: "CityReview");

            migrationBuilder.DropTable(
                name: "CitySetting");

            migrationBuilder.DropTable(
                name: "CityUser");

            migrationBuilder.DropTable(
                name: "CountryCoupen");

            migrationBuilder.DropTable(
                name: "CountryOrder");

            migrationBuilder.DropTable(
                name: "CountryOrderItem");

            migrationBuilder.DropTable(
                name: "CountryProduct");

            migrationBuilder.DropTable(
                name: "CountryProductVarient");

            migrationBuilder.DropTable(
                name: "CountryReview");

            migrationBuilder.DropTable(
                name: "CountrySetting");

            migrationBuilder.DropTable(
                name: "CountryUser");

            migrationBuilder.DropTable(
                name: "CoupenOrderItem");

            migrationBuilder.DropTable(
                name: "CoupenReview");

            migrationBuilder.DropTable(
                name: "CoupenSetting");

            migrationBuilder.DropTable(
                name: "CoupenShippingAddress");

            migrationBuilder.DropTable(
                name: "CoupenState");

            migrationBuilder.DropTable(
                name: "CoupenUser");

            migrationBuilder.DropTable(
                name: "OrderItemReview");

            migrationBuilder.DropTable(
                name: "OrderItemSetting");

            migrationBuilder.DropTable(
                name: "OrderItemShippingAddress");

            migrationBuilder.DropTable(
                name: "OrderItemState");

            migrationBuilder.DropTable(
                name: "OrderItemUser");

            migrationBuilder.DropTable(
                name: "OrderProduct");

            migrationBuilder.DropTable(
                name: "OrderProductVarient");

            migrationBuilder.DropTable(
                name: "OrderReview");

            migrationBuilder.DropTable(
                name: "OrderSetting");

            migrationBuilder.DropTable(
                name: "OrderState");

            migrationBuilder.DropTable(
                name: "ProductSetting");

            migrationBuilder.DropTable(
                name: "ProductShippingAddress");

            migrationBuilder.DropTable(
                name: "ProductState");

            migrationBuilder.DropTable(
                name: "ProductUser");

            migrationBuilder.DropTable(
                name: "ProductVarientReview");

            migrationBuilder.DropTable(
                name: "ProductVarientSetting");

            migrationBuilder.DropTable(
                name: "ProductVarientShippingAddress");

            migrationBuilder.DropTable(
                name: "ProductVarientState");

            migrationBuilder.DropTable(
                name: "ProductVarientUser");

            migrationBuilder.DropTable(
                name: "ReviewSetting");

            migrationBuilder.DropTable(
                name: "ReviewShippingAddress");

            migrationBuilder.DropTable(
                name: "ReviewState");

            migrationBuilder.DropTable(
                name: "SettingShippingAddress");

            migrationBuilder.DropTable(
                name: "SettingState");

            migrationBuilder.DropTable(
                name: "SettingUser");

            migrationBuilder.DropTable(
                name: "StateUser");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "Coupens");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ShippingAddresses");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductVarients");

            migrationBuilder.DropTable(
                name: "Reviews");
        }
    }
}
