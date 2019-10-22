using PasswordSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mini_Project.Models;
using System.Web.Helpers;

namespace Mini_Project.Controllers
{
    public class AccountController : Controller
    {
        //Global declared
        private IMDBSEntities1 db = new IMDBSEntities1();
        private SmsController sms = new SmsController();

        // GET: Account
        public ActionResult Index()
        {
            //using (IMDBSEntities db = new IMDBSEntities)
            {
                return View(db.Accounts.ToList());
            }
        }

        // Registration
        public ActionResult Register()
        {
            Account ua = new Account();
            return View(ua);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Account ua, string PhoneNumber_full)
        {
            if (ModelState.IsValid)
            {
                var username = db.Accounts.SingleOrDefault(m => m.UserName == ua.UserName);
                if (username != null)
                {
                    ModelState.AddModelError("", "This username has been used");
                }
                else
                {

                    //using (IMDBSEntities db = new IMDBSEntities())
                    {
                        //ua.PhoneNumber = "+65" + ua.PhoneNumber;
                        ua.PhoneNumber = PhoneNumber_full;
                        ua.EmailConfirmed = false;
                        ua.Terminated = false;
                        ua.Locked = false;
                        ua.AddCount = 0;
                        //Creating Hash for password
                        ua.Password = PasswordStorage.CreateHash(ua.Password);
                        //Find a way to compare passwords
                        //ua.ConfirmPassword = ua.Password;

                        //Generate activation code for the verification of email account
                        ua.ActivationCode = Guid.NewGuid().ToString();
                        db.Accounts.Add(ua);
                        db.SaveChanges();
                        Session["Register"] = ua.Id.ToString();
                    }
                    ModelState.Clear();
                    //ViewBag.Message = ua.FirstName + " " + ua.LastName + " is successfully registered!";

                    // Sending email confirmation after successfully creating the acount.
                    try
                    {
                        var GenerateUserVerificationLink = "/Account/EmailVerified/" + ua.ActivationCode;
                        var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenerateUserVerificationLink);
                        //Configuring webMail class to send emails
                        //gmail smtp server
                        WebMail.SmtpServer = "smtp.gmail.com";
                        //gmail port to send emails
                        WebMail.SmtpPort = 587;
                        WebMail.SmtpUseDefaultCredentials = true;
                        //sending emails with secure protocol
                        WebMail.EnableSsl = true;
                        //EmailId used to send emails from application
                        WebMail.UserName = "CLTMiniProject@gmail.com";
                        WebMail.Password = "1a2b3c4d!";

                        //Sender email address
                        WebMail.From = "CLTMiniProject@gmail.com";

                        //Send email
                        WebMail.Send(to: ua.UserName, subject: "Email Verification", body: "<p>Welcome!</p>" + "<p>Thank you for signing up! We just need you to verify your email address to complete setting up your account. </p>" + "<p>Please click the link below for account verification</p>" + "<br /><br /><a href=" + link + ">" + link + "</a>", isBodyHtml: true);

                    }
                    catch (Exception)
                    {
                        ViewBag.Status = "Problem while sending email, please check details";
                    }
                    return RedirectToAction("Registered");
                }
            }
            return View();
        }

        public ActionResult Registered()
        { if (Session["Register"] != null)
            {
                Account account = new Account();
                return View(account);
            }
            else
            {
                return RedirectToAction("LogIn");
            }
        }

        //Request of Email for Registration
        public ActionResult RequestEmail(Account email)
        {
            if (Session["Register"] != null)
            {
                //Find the corresponding Id of the account from the database and return the details
                int id = int.Parse(Session["Register"].ToString());
                email = db.Accounts.FirstOrDefault(u => u.Id == id);

                if (email != null)
                {
                    try
                    {
                        var GenerateUserVerificationLink = "/Account/EmailVerified/" + email.ActivationCode;
                        var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenerateUserVerificationLink);
                        //Configuring webMail class to send emails
                        //gmail smtp server
                        WebMail.SmtpServer = "smtp.gmail.com";
                        //gmail port to send emails
                        WebMail.SmtpPort = 587;
                        WebMail.SmtpUseDefaultCredentials = true;
                        //sending emails with secure protocol
                        WebMail.EnableSsl = true;
                        //EmailId used to send emails from application
                        WebMail.UserName = "CLTMiniProject@gmail.com";
                        WebMail.Password = "1a2b3c4d!";

                        //Sender email address
                        WebMail.From = "CLTMiniProject@gmail.com";

                        //Send email
                        WebMail.Send(to: email.UserName, subject: "Email Verification", body: "<p>Welcome!</p>" + "<p>Thank you for signing up! We just need you to verify your email address to complete setting up your account. </p>" + "<p>Please click the link below for account verification</p>" + "<br /><br /><a href=" + link + ">" + link + "</a>", isBodyHtml: true);

                    }
                    catch (Exception)
                    {
                        ViewBag.Status = "Problem while sending email, please check details";
                    }
                    return RedirectToAction("Registered");
                }
            }
            else
            {
                return RedirectToAction("LogIn");
            }
            return View();
        }


        //Email verified
        public ActionResult EmailVerified(string id)
        {
            if (id != null) {
                db.Configuration.ValidateOnSaveEnabled = true;
                var IsVerify = db.Accounts.Where(u => u.ActivationCode == new Guid(id).ToString()).FirstOrDefault();

                if (IsVerify != null && IsVerify.EmailConfirmed == false)
                {
                    IsVerify.EmailConfirmed = true;
                    db.SaveChanges();
                    ViewBag.Message = "Congratulations! Your email verification is completed! You are now logged in and may use your account.";
                    Session["Password"] = IsVerify.Password.ToString();
                    Session["Firstname"] = IsVerify.FirstName.ToString();
                    Session["Id"] = IsVerify.Id.ToString();
                    Session["Terminated"] = IsVerify.Terminated.ToString();
                    Session["ActivationCode"] = IsVerify.ActivationCode.ToString();
                    Session["EmailConfirmed"] = IsVerify.EmailConfirmed.ToString();
                    Session["Locked"] = IsVerify.Locked.ToString();
                    Session["AddCount"] = IsVerify.AddCount.ToString();
                }
                else if (IsVerify != null && IsVerify.EmailConfirmed == true)
                {
                    Session.Abandon();
                    ViewBag.Message = "Your email has been verified already! Please press the settings and use your account.";
                }
                else
                {
                    ViewBag.Message = "Invalid Request... Email not verified";
                }
            } else
            {
                return RedirectToAction("LogIn");
            }
            
            return View();
        }

        //Login
        public ActionResult LogIn()
        {
            if (Session["Id"] != null)
            {
                return RedirectToAction("Manage");
            }
            else
            {
                Account user = new Account();
                return View(user);
            }
        }

        [HttpPost]
        public ActionResult LogIn(Account user)
        {
            var usr = db.Accounts.FirstOrDefault(u => u.UserName == user.UserName); 
            {
                //using (IMDBSEntities db = new IMDBSEntities())
                {
                    //Check if the email that is typed into the website is the same as that in the database
                    //var usr = db.Accounts.FirstOrDefault(u => u.UserName == user.UserName);

                    if (usr != null)
                    {
                        //if the emails match, then continue
                        if (usr.UserName == user.UserName && usr.Terminated == false)
                        {
                            if (usr.EmailConfirmed == true && usr.Locked == false)
                            {
                                //pwd will convert the typed password in the website to hashed key and verify with the hashed key in the database
                                var pwd = PasswordStorage.VerifyPassword(user.Password, usr.Password);
                                if (pwd != false)
                                {
                                    Session["Password"] = usr.Password.ToString();
                                    Session["Id"] = usr.Id.ToString();
                                    Session["Username"] = usr.UserName.ToString();
                                    Session["Firstname"] = usr.FirstName.ToString();
                                    Session["Terminated"] = usr.Terminated.ToString();
                                    Session["ActivationCode"] = usr.ActivationCode.ToString();
                                    Session["EmailConfirmed"] = usr.EmailConfirmed.ToString();
                                    Session["Locked"] = usr.Locked.ToString();
                                    Session["AddCount"] = usr.AddCount.ToString();
                                    usr.AddCount = 0;
                                    db.Entry(usr).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                    return RedirectToAction("Manage", "Account");
                                }
                                //else
                                //{
                                //    ModelState.AddModelError("", "Username/Password is incorrect");
                                //}

                                else
                                {
                                    usr.AddCount += 1;
                                    if (usr.AddCount < 5)
                                    {
                                            var counter = 5 - usr.AddCount;
                                            db.Entry(usr).State = System.Data.Entity.EntityState.Modified;
                                            db.SaveChanges();
                                            ModelState.AddModelError("", $"Username/Password is incorrect. You have {counter} more tries.");
                                    }
                                    else
                                    {
                                        usr.Locked = true;
                                        db.Entry(usr).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        ModelState.AddModelError("", "Your account has been locked. Please contact your administrator to unlock");
                                    }
                                }
                      
                         }
                            else
                            {
                                var pwd = PasswordStorage.VerifyPassword(user.Password, usr.Password);
                                if (pwd != false)
                                {
                                    Session["Id"] = usr.Id.ToString();
                                    Session["Firstname"] = usr.FirstName.ToString();
                                    return RedirectToAction("VerifyPlease");
                                }
                                else
                                {
                                    usr.AddCount += 1;
                                    if (usr.AddCount < 5)
                                    {
                                        var counter = 5 - usr.AddCount;
                                        db.Entry(usr).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        ModelState.AddModelError("", $"Username/Password is incorrect. You have {counter} more tries.");
                                    }
                                    else
                                    {
                                        usr.Locked = true;
                                        db.Entry(usr).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        ModelState.AddModelError("", "Your account has been locked. Please contact your administrator to unlock");
                                    }
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid username or password");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username/Password is incorrect");
                    }
                }
            }
            return View();
        }

        //If email is not verified yet
        public ActionResult VerifyPlease()
        {
            if (Session["Id"] != null)
            {
                Account account = new Account();
                int id = int.Parse(Session["Id"].ToString());
                account = db.Accounts.FirstOrDefault(m => m.Id == id);

                if (account.EmailConfirmed == false)
                {
                    return View(account);
                }
                else
                {
                    return RedirectToAction("Locked");
                }
            }
            else
            {
                return RedirectToAction("LogIn");
            }
        }

        public ActionResult Locked()
        {
            if (Session["Id"] != null)
            {
                Session.Abandon();
                return View();
            }
            return View();
        }

        public ActionResult VerifyAgain()
        {
            if (Session["Id"] != null)
            {
                Account account = new Account();
                int id = int.Parse(Session["Id"].ToString());
                account = db.Accounts.FirstOrDefault(m => m.Id == id);

                if (account.EmailConfirmed == false)
                {
                    return View(account);
                }
                else
                {
                    Session.Abandon();
                    return RedirectToAction("LogIn");
                }
            }
            else
            {
                return RedirectToAction("LogIn");
            }
        }

        //Request of Email again for Registration
        public ActionResult RequestEmailAgain(Account email)
        {
            if (Session["Id"] != null)
            {
                //Find the corresponding Id of the account from the database and return the details
                int id = int.Parse(Session["Id"].ToString());
                email = db.Accounts.FirstOrDefault(u => u.Id == id);

                if (email != null)
                {
                    if (email.EmailConfirmed == false)
                    {
                        email.ActivationCode = Guid.NewGuid().ToString();
                        db.Entry(email).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        try
                        {
                            var GenerateUserVerificationLink = "/Account/EmailVerified/" + email.ActivationCode;
                            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenerateUserVerificationLink);
                            //Configuring webMail class to send emails
                            //gmail smtp server
                            WebMail.SmtpServer = "smtp.gmail.com";
                            //gmail port to send emails
                            WebMail.SmtpPort = 587;
                            WebMail.SmtpUseDefaultCredentials = true;
                            //sending emails with secure protocol
                            WebMail.EnableSsl = true;
                            //EmailId used to send emails from application
                            WebMail.UserName = "CLTMiniProject@gmail.com";
                            WebMail.Password = "1a2b3c4d!";

                            //Sender email address
                            WebMail.From = "CLTMiniProject@gmail.com";

                            //Send email
                            WebMail.Send(to: email.UserName, subject: "Email Verification", body: "<p>Welcome!</p>" + "<p>Thank you for signing up! We just need you to verify your email address to complete setting up your account. </p>" + "<p>Please click the link below for account verification</p>" + "<br /><br /><a href=" + link + ">" + link + "</a>", isBodyHtml: true);
                        }
                        catch (Exception)
                        {
                            ViewBag.Status = "Problem while sending email, please check details";
                        }
                        return RedirectToAction("VerifyAgain");
                    }
                    else
                    {
                        return RedirectToAction("Manage");
                    }
                }
            }
            return View();
        }

        //Logged In
        public ActionResult LoggedIn()
        {
            if (Session["Password"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("LogIn");
            }
        }

        //Log Out
        public ActionResult LogOut()
        {
            if (Session["Id"] != null)
            {
                Session.Abandon();
                return RedirectToAction("LogOut");
            }
            else
            {
                return RedirectToAction("LogIn");
            }

        }

        //Change of password
        public ActionResult ChangePassword()
        {
            if (Session["Id"] != null)
            {
                Account account = new Account();
                return View(account);
            }
            else
            {
                return RedirectToAction("LogIn");
            }
            
        }

        [HttpPost]
        public ActionResult ChangePassword(Account account)
        {
            Account acct = new Account();
            int id = int.Parse(Session["Id"].ToString());
            acct = db.Accounts.FirstOrDefault(m => m.Id == id);

            //Hash new passwords
            acct.Password = PasswordStorage.CreateHash(account.Password);
            //acct.ConfirmPassword = acct.Password;

            //Update the database with new passwords
            //Update account, using updatemodel() will not be able update the database as the context is disposed and the model will be detached from the database.
            db.Entry(acct).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("PasswordChanged");
        }

        //Reset of Password
        public ActionResult ResetPassword()
        {
            Account pw = new Account();
            return View(pw);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(Account pw)
        {
            //if (ModelState.IsValid)
            {
                //using (IMDBSEntities db = new IMDBSEntities())
                {
                    //Check if the email that is typed into the website is the same as that in the database
                    var user_email = db.Accounts.FirstOrDefault(p => p.UserName == pw.UserName);
                    
           
                    if (user_email != null && user_email.UserName == pw.UserName)
                    {
                        //Generate OTP code for the verification of email account
                        int otpValue = new Random().Next(100000, 999999);
                        user_email.otpValuePasswordReset = otpValue;

                        db.Entry(user_email).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Session["id"] = user_email.Id.ToString();

                        //Sending of OTP to email
                        try
                        {
                            //Configuring webMail class to send emails
                            //gmail smtp server
                            WebMail.SmtpServer = "smtp.gmail.com";
                            //gmail port to send emails
                            WebMail.SmtpPort = 587;
                            WebMail.SmtpUseDefaultCredentials = true;
                            //sending emails with secure protocol
                            WebMail.EnableSsl = true;
                            //EmailId used to send emails from application
                            WebMail.UserName = "CLTMiniProject@gmail.com";
                            WebMail.Password = "1a2b3c4d!";

                            //Sender email address
                            WebMail.From = "CLTMiniProject@gmail.com";

                            //Send email
                            WebMail.Send(to: user_email.UserName, subject: "Reset of Password", body: "Hey " + user_email.FirstName + "," +
                                "<p>You have requested to reset your password. Please verify by entering the given One Time Password.</p>" + "<p>Your OTP password is " + user_email.otpValuePasswordReset, isBodyHtml: true);
                        }
                        catch (Exception)
                        {
                            ViewBag.Status = "Problem while sending email, please check details";
                        }

                        return RedirectToAction("VerifyPasswordOTP");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email address is not registered.");
                    }
                    
                }


            }
            return View();
        }

        //Verifying of OTP for password reset
        public ActionResult VerifyPasswordOTP()
        {
            if (Session["id"] != null)
            {
                Account pOTP = new Account();
                return View(pOTP);
            }
            else
            {
                return RedirectToAction("LogIn");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyPasswordOTP(Account pOTP)
        {
            //if (ModelState.IsValid)
            {
                //using (IMDBSEntities db = new IMDBSEntities())
                {
                    //Check if the OTP password that is typed into the website is the same as that in the database
                    var otpPW = db.Accounts.FirstOrDefault(p => p.otpValuePasswordReset == pOTP.otpValuePasswordReset);

                    //if the OTP passwords matched, then continue
                    if (otpPW != null)
                    {
                        //Creating tempdata
                        Account acct = new Account();
                        acct.Id = otpPW.Id;
                        TempData["user"] = acct;
                        return RedirectToAction("ResetNewPassword");
                    }
                    else
                    {
                        ModelState.AddModelError("", "OTP password is incorrect");
                    }
                }
            }
            return View();
        }


        //Request of Email again for password reset
        public ActionResult RequestEmailPassword(Account email)
        {
            if (Session["id"] != null)
            {
                //Find the corresponding Id of the account from the database and return the details
                int id = int.Parse(Session["id"].ToString());
                email = db.Accounts.FirstOrDefault(u => u.Id == id);

                if (email != null)
                {
                    try
                    {
                        //Generate OTP code for the verification of email account
                        int otpValue = new Random().Next(100000, 999999);
                        email.otpValuePasswordReset = otpValue;

                        db.Entry(email).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        //Configuring webMail class to send emails
                        //gmail smtp server
                        WebMail.SmtpServer = "smtp.gmail.com";
                        //gmail port to send emails
                        WebMail.SmtpPort = 587;
                        WebMail.SmtpUseDefaultCredentials = true;
                        //sending emails with secure protocol
                        WebMail.EnableSsl = true;
                        //EmailId used to send emails from application
                        WebMail.UserName = "CLTMiniProject@gmail.com";
                        WebMail.Password = "1a2b3c4d!";

                        //Sender email address
                        WebMail.From = "CLTMiniProject@gmail.com";

                        //Send email
                        WebMail.Send(to: email.UserName, subject: "Reset of Password", body: "Hey " + email.FirstName + "," +
                                "<p>You have requested to reset your password. Please verify by entering the given One Time Password.</p>" + "<p>Your OTP password is " + email.otpValuePasswordReset, isBodyHtml: true);

                    }
                    catch (Exception)
                    {
                        ViewBag.Status = "Problem while sending email, please check details";
                    }
                    return RedirectToAction("VerifyPasswordOTP");
                }
                else
                {
                    return RedirectToAction("LogIn");
                }
            }
            else
            {
                return RedirectToAction("LogIn");
            }
        }

        //Follow up for password reset password
        public ActionResult ResetNewPassword()
        {
            if (Session["id"] != null)
            {
                Account newPW = (Account)TempData["user"];
                return View(newPW);
            }
            else
            {
                return RedirectToAction("LogIn");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetNewPassword(Account newPW)
        {
            //if (ModelState.IsValid)
            {
                //using (IMDBSEntities db = new IMDBSEntities())
                {
                    var user_email = db.Accounts.FirstOrDefault(p => p.Id == newPW.Id);

                    //Hash new passwords
                    user_email.Password = PasswordStorage.CreateHash(newPW.Password);
                    //user_email.ConfirmPassword = user_email.Password;

                    //Update the database with new passwords
                    //UpdateModel(user_email, "Account");
                    db.Entry(user_email).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("PasswordReset");
            }
            //return View();
        }

        public ActionResult PasswordReset()
        { 
            if (Session["id"] != null)
            {
               Session.Abandon();
               return View();
            }
           else
            {
                return RedirectToAction("LogIn");
            }
        }

        public ActionResult PasswordChanged()
        {
            if (Session["Id"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("LogIn");
            }
            
        }

        //Request of Username
        public ActionResult CheckPhoneNumber()
        {
            Account number = new Account();
            return View(number);
        }

        [HttpPost]
        public ActionResult CheckPhoneNumber(Account number)
        {
            //Check if phone number is valid
            //if(ModelState.IsValid)
            {
                number.PhoneNumber = "+65" + number.PhoneNumber;
                var user_number = db.Accounts.FirstOrDefault(n => n.PhoneNumber == number.PhoneNumber);

                if(user_number != null)
                {
                    int otpValue = new Random().Next(100000, 999999);

                    user_number.otpValueUserName = otpValue;
                    Session["OTP"] = user_number.Id.ToString();
                    //UpdateModel(user_number, "Account");
                    db.Entry(user_number).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    sms.SendSms("Hello, " + user_number.FirstName + ". Your OTP is " + user_number.otpValueUserName, user_number.PhoneNumber);

                    return RedirectToAction("VerifyOTP");
                }
            else
            {
                 ModelState.AddModelError("", "Phone number is not registered.");
            }
        }
            return View();
        }

        public ActionResult VerifyOTP()
        { if (Session["OTP"] != null)
            {
                Account otp = new Account();
                return View(otp);
            }
            else
            {
                return RedirectToAction("LogIn");
            }
        }

        [HttpPost]
        public ActionResult VerifyOTP(Account otp)
        {
            //if (ModelState.IsValid)
            {
                var user_otp = db.Accounts.FirstOrDefault(o => o.otpValueUserName == otp.otpValueUserName);

                if (user_otp != null)
                {
                    sms.SendSms("Hello, " + user_otp.FirstName + ". Your user name is " + user_otp.UserName, user_otp.PhoneNumber);
                    return RedirectToAction("RecoverUserName");
                }
                else
                {
                    ModelState.AddModelError("", "The OTP that you have typed in is incorrect.");
                }
                return View();
            }
            //return View();
        }


        public ActionResult RequestOTP(Account otp)
        {
            if (Session["OTP"] != null)
            {
                //Find the corresponding Id of the account from the database and return the details
                int id = int.Parse(Session["OTP"].ToString());
                otp = db.Accounts.FirstOrDefault(u => u.Id == id);

                if (otp != null)
                {
                    int otpValue = new Random().Next(100000, 999999);

                    otp.otpValueUserName = otpValue;


                    //UpdateModel(user_number, "Account");
                    db.Entry(otp).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    sms.SendSms("Hello, " + otp.FirstName + ". Your OTP is " + otp.otpValueUserName, otp.PhoneNumber);
                    return RedirectToAction("VerifyOTP");
                }
                else
                {
                    return RedirectToAction("LogIn");
                }
            }
            else
            {
                return RedirectToAction("LogIn");
            }
            
        }

        public ActionResult RecoverUserName()
        {
            if (Session["OTP"] != null)
            {
                Account user = new Account();
                Session.Abandon();
                return View(user);
            }
            else
            {
                return RedirectToAction("LogIn");
            }
           
        }

        //Manage account
        public ActionResult Manage()
        {
            if (ModelState.IsValid)
            {
                Account account = new Account();
                if (Session["Id"] != null)
                {
                    //Find the corresponding Id of the account from the database and return the details
                    int id = int.Parse(Session["Id"].ToString());
                    account = db.Accounts.FirstOrDefault(u => u.Id == id);

                    if (account.EmailConfirmed != false)
                    {
                        return View(account);
                    }
                    else
                    {
                        return RedirectToAction("VerifyPlease");
                    }
                }
                else
                {
                    return RedirectToAction("LogIn");
                }
            }
            return View();
        }

        //Edit of account
        public ActionResult Edit()
        {
            if (Session["Id"] != null)
            {
                Account account = new Account();
                int id = int.Parse(Session["Id"].ToString());
                account = db.Accounts.FirstOrDefault(m => m.Id == id);
                return View(account);
            }
            else
            {
                return RedirectToAction("LogIn");
            }
            
        }
        
        [HttpPost]
        public ActionResult Edit(Account account)
        {
           //if (ModelState.IsValid)
            {
                //Update account, using updatemodel() will not be able update the database as the context is disposed and the model will be detached from the database.
                account.Password = Session["Password"].ToString();
                //account.ConfirmPassword = account.Password;
                account.Terminated = bool.Parse(Session["Terminated"].ToString());
                account.ActivationCode = Session["ActivationCode"].ToString();
                account.EmailConfirmed = bool.Parse(Session["EmailConfirmed"].ToString());
                account.Locked = bool.Parse(Session["Locked"].ToString());
                account.AddCount = int.Parse(Session["AddCount"].ToString());
                db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Manage");
        }

        //Termination of account
        public ActionResult Terminate()
        {
            if(ModelState.IsValid)
            {
                Account account = new Account();
                
                if (Session["Id"] != null)
                {
                    int id = int.Parse(Session["Id"].ToString());
                    account = db.Accounts.FirstOrDefault(m => m.Id == id);
                    account.Terminated = true;
                    UpdateModel(account, "Account");
                    db.SaveChanges();
                    Session.Abandon();
                    return View("Terminate");
                }
                else
                {
                    return RedirectToAction("LogIn");
                }
            }
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }
    }
}