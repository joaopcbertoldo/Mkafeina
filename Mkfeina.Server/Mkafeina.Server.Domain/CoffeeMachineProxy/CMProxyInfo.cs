﻿using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public class CMProxyInfo
	{
		public const string
			ENABLED = "enabled",
			REGISTRATION = "registration",
			MAKING_COFFEE = "makingCoffee";

		#region Internal Stuff

		private Dictionary<string, Ingredient> _ingredients;
		private string _uniqueName;
		private bool _makingCoffee;
		private bool _enabled;
		private CMProxy _owner;

		#endregion Internal Stuff

		internal CMProxyInfo(RegistrationRequest request, CMProxy owner)
		{
			_ingredients = new Dictionary<string, Ingredient>(Ingredient.GetAllExistingIngredientsInstances());
			Mac = request.mac;
			UniqueName = request.un;
			_owner = owner;
			SetupAvaiabilityAndOffsets(request.stp);
		}

		public void SetupAvaiabilityAndOffsets(IngredientsSetup setup)
		{
			var ingredients = Ingredient.GetAllExistingIngredientsNames().Select(name => Ingredient.GetCode(name)).ToList();
			ingredients.ForEach(iCode =>
			{
				var isAvailable = (bool)typeof(IngredientsSetup).GetProperty($"{iCode}a").GetValue(setup);
				if (isAvailable)
				{
					var empty = (float)typeof(IngredientsSetup).GetProperty($"{iCode}e").GetValue(setup);
					var full = (float)typeof(IngredientsSetup).GetProperty($"{iCode}f").GetValue(setup);
					SetupIngredient(Ingredient.GetName(iCode), isAvailable, empty, full);
				}
				else
					SetupIngredient(Ingredient.GetName(iCode), false);
			});
		}

		public void SetupIngredient(string ingredientName, bool available, float? emptyOffset = null, float? fullOffset = null)
		{
			_ingredients[ingredientName].Available = available;
			_ingredients[ingredientName].EmptyOffset = emptyOffset;
			_ingredients[ingredientName].FullOffset = fullOffset;
		}

		public IEnumerable<string> AllRecipesNames { get => _owner.Cookbook.AllRecipesNames; }

		internal bool HasRecipe(string recipeName) => _owner.Cookbook[recipeName] == null ? false : true;

		public IEnumerable<string> AvailableIngredients { get => _ingredients.Where(i => i.Value.Available).Select(i => i.Value.Name).ToList(); }

		public bool LevelsAreUnderMinimum { get => _ingredients.Values.Any(i => i.Level < i.MinimumLevel); }

		public string Mac { get; set; }

		public string UniqueName {
			get => _uniqueName;
			set {
				_uniqueName = value;
			}
		}

		public bool Enabled {
			get => _enabled;
			set {
				_enabled = value;
				_owner.OnChangeEvent(ENABLED);
			}
		}

		public bool MakingCoffee {
			get => _makingCoffee;
			set {
				_makingCoffee = value;
				_owner.OnChangeEvent(MAKING_COFFEE);
			}
		}

		public int? GetLevel(string ingredientName) => _ingredients[ingredientName].Level;

		public void UpdateIngredients(IngredientsSignals signals)
		{
			_ingredients.Values
						.ToList()
						.ForEach(i =>
						{
							i.Signal = (float)typeof(IngredientsSignals).GetProperty(i.Code.ToString()).GetValue(signals);
							_owner.OnChangeEvent(i.Name);
						});
		}
	}
}