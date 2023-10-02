﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MvcTraining.Models;
using MvcTraining.Repositories.Recipe;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MvcTraining.Controllers
{
    public class RecipeController : BaseController
    {
        private readonly RecipeService _recipeService;
        private readonly IngredientService _ingredientService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RecipeController(RecipeService recipeService,IngredientService ingredientService,
                                IWebHostEnvironment webHostEnvironment)
        {
            _recipeService = recipeService;
            _ingredientService = ingredientService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult GetAllView()
        {
            string successMessage = TempData["SuccessMessage"] as string;
            ViewData["SuccessMessage"] = successMessage;
            return View();
        }
        public IActionResult Create()
        {
            RecipeModel recipe = new RecipeModel();
            return View(recipe);
        }

        [HttpPost]
        public IActionResult Save(RecipeModel recipe,List<string> ingredientName,List<string> ingredientQuantity,
                                    List<string> ingredientUnit)
        {
            if (ModelState.IsValid)
            {
                string folder = "dish_photo/";
                folder += Guid.NewGuid().ToString() + "_" + recipe.PhotoUrl.FileName;
                string serverFolder=Path.Combine(_webHostEnvironment.WebRootPath, folder);

                recipe.DishPhoto = "/" + folder; 

                RecipeDto recipeDto = ChangeModel.Change(recipe);
                long saveRecipe = _recipeService.SaveRecipe(recipeDto);
                if(ingredientName != null && saveRecipe>0)
                {
                    for(var i = 0; i < ingredientName.Count; i++)
                    {
                        IngredientDto ingreDto=new IngredientDto();
                        ingreDto.Name = ingredientName[i];
                        ingreDto.Quantity =Convert.ToInt16(ingredientQuantity[i]);
                        ingreDto.Unit = ingredientUnit[i];
                        int ingreSaveResult = _ingredientService.SaveIngredient(ingreDto, saveRecipe);
                    }
                }
                recipe.PhotoUrl.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                TempData["SuccessMessage"] = "Saved successful";
                return RedirectToAction("GetAllView", "Recipe");
            }
            ViewData["IngredientName"] = JsonConvert.SerializeObject(ingredientName);
            ViewData["IngredientQuantity"] = JsonConvert.SerializeObject(ingredientQuantity);
            ViewData["IngredientUnit"] = JsonConvert.SerializeObject(ingredientUnit);
            return View("Create",recipe);
        }

        [HttpPost]
        public IActionResult GetAll()
        {
            var requestData = GetFormRequest();
            var recipeList=_recipeService.GetAll(requestData);
            return ToJson(requestData.Draw,recipeList.RequestTotal,recipeList.RequestFilter,recipeList.recipes);
        }
    }
}