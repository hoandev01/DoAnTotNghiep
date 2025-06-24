﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChickenF.Migrations
{
    /// <inheritdoc />
    public partial class AddBigUpdateAboutOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Orders");
        }
    }
}
