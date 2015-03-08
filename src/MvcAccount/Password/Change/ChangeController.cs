﻿// Copyright 2012 Max Toro Q.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using MvcAccount.Shared;
using MvcCodeRouting.Web.Mvc;
using ResultEnvelope.Web.Mvc;

namespace MvcAccount.Password.Change {

   /// <summary>
   /// Exposes password change functionality.
   /// </summary>
   [Authorize]
   public class ChangeController : BaseController {

      AccountRepository repo;
      PasswordService passServ;

      PasswordChanger changer;

      /// <summary>
      /// Initializes a new instance of the <see cref="ChangeController"/> class.
      /// </summary>
      public ChangeController() { }

      /// <summary>
      /// Initializes a new instance of the <see cref="ChangeController"/> class, 
      /// with the provided <paramref name="repo"/> and <paramref name="passwordService"/>.
      /// </summary>
      /// <param name="repo">The account repository.</param>
      /// <param name="passwordService">The password service.</param>
      public ChangeController(AccountRepository repo, PasswordService passwordService) 
         : this() {
         
         this.repo = repo;
         this.passServ = passwordService;
      }

      /// <summary>
      /// Initializes data that might not be available when the constructor is called.
      /// </summary>
      /// <param name="requestContext">The HTTP context and route data.</param>
      protected override void Initialize(RequestContext requestContext) {
         
         base.Initialize(requestContext);

         this.changer = new PasswordChanger(this.Configuration, this, this.repo, this.passServ);
      }

      /// <summary>
      /// The change password page.
      /// </summary>
      /// <returns>The action result.</returns>
      [HttpGetHead]
      [DefaultAction]
      public ActionResult Change() {

         this.ViewData.Model = new ChangeViewModel(new ChangeInput());

         return View();
      }

      /// <summary>
      /// Attempts to change the password.
      /// </summary>
      /// <param name="input">The input model.</param>
      /// <param name="cancel">A value that indicates if the operation was cancelled by the user.</param>
      /// <returns>The action result.</returns>
      [HttpPost]
      public ActionResult Change(ChangeInput input, FormButton cancel) {

         if (cancel) {
            return Redirect(this.Url.Action("", "~Account"));
         }

         this.ViewData.Model = new ChangeViewModel(input);

         if (!this.ModelState.IsValid) {
            return View().WithStatus(HttpStatusCode.BadRequest);
         }

         var result = this.changer.Change(input);

         if (result.IsError) {
            return View().WithErrors(result);
         }

         return Redirect(this.Url.Action(Saved));
      }

      /// <summary>
      /// A page that informs the user that his new password was saved.
      /// </summary>
      /// <returns>The action result.</returns>
      [HttpGetHead]
      public ActionResult Saved() {

         this.ViewData.Model = new SavedViewModel();

         return View();
      }
   }
}