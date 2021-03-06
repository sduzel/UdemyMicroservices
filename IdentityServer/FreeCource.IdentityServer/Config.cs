// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace FreeCource.IdentityServer
{
    public static class Config // kimler token alabilir, hangi mslere istek yapıldığında token verileceğini belirtir.
    {
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("resource_catalog"){ Scopes = {"catalog_fullpermission" } },
            new ApiResource("resource_photo_stock"){ Scopes = { "photos_stock_fullpermission" } },
            new ApiResource("resource_basket"){ Scopes = { "basket_fullpermission" } },
            new ApiResource("resource_discount"){ Scopes = { "discount_fullpermission" } },
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
        };

        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.Email(),
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource(){
                Name = "roles",
                DisplayName = "Roles",
                Description = "Kullanıcı rolleri",
                UserClaims = new []{"role"} 
            }
        };

        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
            new ApiScope("catalog_fullpermission","CatalogAPI için full erişim"),
            new ApiScope("photos_stock_fullpermission","PhotoStockAPI için full erişim"),
            new ApiScope("basket_fullpermission","BasketAPI için full erişim"),
            new ApiScope("discount_fullpermission","DiscountAPI için full erişim"),
            new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
        };

        public static IEnumerable<Client> Clients => new Client[]
        {
            new Client
            {
                ClientName = "Asp.Net Core MVC",
                ClientId = "WebMvcClient",
                ClientSecrets = { new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { 
                    "catalog_fullpermission", 
                    "photos_stock_fullpermission", 
                    "discount_fullpermission",
                    IdentityServerConstants.LocalApi.ScopeName 
                }

            },
            new Client
            {
                ClientName = "Asp.Net Core MVC",
                ClientId = "WebMvcClientForUser",
                AllowOfflineAccess = true,
                ClientSecrets = { new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = {
                    "basket_fullpermission","discount_fullpermission",
                    IdentityServerConstants.StandardScopes.Email, 
                    IdentityServerConstants.StandardScopes.OpenId, 
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess, // kullanıcı offline olsada elimizde refresh token ile yeni bir token almamızı sağlıyor.
                    "roles",
                    IdentityServerConstants.LocalApi.ScopeName
                },
                AccessTokenLifetime = 1*60*60, // 1 saat,her token alındığında yeni bir refresh token alınır yeni 60 gün verilir
                RefreshTokenExpiration = TokenExpiration.Absolute,
                AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,//60gün 
                RefreshTokenUsage = TokenUsage.ReUse
            }
        };
    }
}