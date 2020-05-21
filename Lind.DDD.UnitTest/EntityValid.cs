using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Lind.DDD.EntityValidation;

namespace Lind.DDD.UnitTest
{
    class UserModel : Lind.DDD.Domain.Entity
    {
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Range(0, 100)]
        public decimal Money { get; set; }
    }
    [TestClass]
    public class EntityValid
    {

        [TestMethod]
        public void Email()
        {
            var user = new UserModel()
            {
                Email = "zzl",
                Money = -10,
                Name = null
            };
            var result = user.GetRuleViolations();
            var valid = new ValidateProvider(user);
            if (!valid.IsValid)
                Console.WriteLine(string.Join(",", valid.GetErrorMessages()));
        }
    }
}
