﻿using IdentityMVCCore.EditModel;
using IdentityMVCCore.Models;
using IdentityMVCCore.Security;
using IdentityMVCCore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityMVCCore.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IDataProtector protector;
        private readonly ILogger<EmployeeController> logger;

        public EmployeeController(IDataProtectionProvider protectionProvider, DataProtectorPurposeString purposeString, ILogger<EmployeeController> logger)
        {
            this.protector = protectionProvider.CreateProtector(purposeString.EmployeeIdRoutueValue);
            this.logger = logger;
        }
        [HttpGet]
        
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
       
        public IActionResult Create(EmployeeViewModel model)
        {
            if(ModelState.IsValid)
            {
                using(EmployeeContext context = new EmployeeContext())
                {
                    Employee emp = new Employee()
                    {
                        Firstname = model.Firstname,
                        Lastname = model.Lastname,
                        Salary = model.Salary,
                        Address = model.Address
                    };
                    context.Employees.Add(emp);
                    context.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "An error has occured while creating employee");
            return View(model);
        }
        public ActionResult List()
        {
            using(EmployeeContext context = new EmployeeContext())
            {
                List<Employee> employee = context.Employees.ToList();
                foreach(Employee emp in employee)
                {
                    emp.EncryptedId = protector.Protect(emp.Id.ToString());
                }
                return View(employee);
            }
        }
        [HttpGet]
        public IActionResult Details(string id)
        {
             int decryptedId = Convert.ToInt32(protector.Unprotect(id));
            using(EmployeeContext context = new EmployeeContext())
            {
                var employee = context.Employees.Where(emp => emp.Id == decryptedId).FirstOrDefault();
                return View(employee);
            }
        }

        [HttpGet]
        
        public IActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        
        public IActionResult Edit(EmployeeEditModel model)
        {
            if(ModelState.IsValid)
            {
                using(EmployeeContext context = new EmployeeContext())
                {
                    var employee = context.Employees.FirstOrDefault(emp => emp.Id == model.Id);
                    if (employee == null)
                    {
                        ModelState.AddModelError("", "Employee with id "+ model.Id.ToString() + " do not exists");
                        return View(model);
                    }
                    else
                    {
                        employee.Firstname = model.Firstname;
                        employee.Lastname = model.Lastname;
                        employee.Salary = model.Salary;
                        employee.Address = model.Address;
                        context.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }

                }
                
            }
            ModelState.AddModelError("", "Error occured while updating employee");
            return View(model); 
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            using(EmployeeContext context = new EmployeeContext())
            {
               
                var employee = (Employee)context.Employees.FirstOrDefault(emp=>emp.Id.Equals(id));
                if(employee == null)
                {
                    ViewBag.ErrorMessage = "Employee with id : " + id + " not found";
                    return View("Error");
                }
                return View(employee);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using(EmployeeContext context = new EmployeeContext())
            {
                var employee = context.Employees.Find(id);
                if (employee == null)
                {
                    return NotFound();
                }

                context.Employees.Remove(employee);
                context.SaveChanges(); // Save the changes to the database

                return RedirectToAction(nameof(Index)); // Redirect back to the employee list
            }
        }

    }
}
