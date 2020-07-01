using System;
using System.Security.Claims;
using DattingApp.api.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

/*
    classe statique contenant des méthodes utilisées par le programme
 */               


namespace DattingApp.api.helpers
{

    public static class Helpers
    {
        public static void AddApplicationHelper(this HttpResponse response,string message)
        {
            response.Headers.Add("Application-Error",message);
            response.Headers.Add("Access-Control-Expose-Headers","Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin","*");
        }

        // methode statique pour ajouter les éléments au header
        // this HttpResponse this--> appel à travers un élélment "Response.AddPagination"  
    public static void AddPagination(this HttpResponse response,
         int current, int itemsPerPage, int totalItems, int totalPages)
        {
            // création instance de PaginationHeader 
            var paginationHeader = new PaginationHeader(current,itemsPerPage,totalItems,totalPages);
            // ajout de l'objet PaginationHeader au Header de la requête
                // pour appliquer du CamelCase
            var camelCaseFormatter = new JsonSerializerSettings();
                //
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.
                    SerializeObject(paginationHeader,camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
        public static int CalcAge(this DateTime theDateTime)// this DateTime theDateTime-->appel à travers objet
            // dans AutoMapperProfiles : =>src.DateOfBirth.CalcAge() --- src pointe sur User
        {
            var age = DateTime.Today.Year - theDateTime.Year;
            // si l'année de naissance + age > date actuelle anniversaire pas encore arrivé on decrémente
            if (theDateTime.AddYears(age) > DateTime.Today)
                age--;
            return age;
        }        
   }

}



  