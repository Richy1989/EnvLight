using System;
using System.Net;
using nanoFramework.WebServer;

namespace NFApp1.WebContent
{
    /// <summary>
    /// The API controller
    /// </summary>
    //[Authentication("Basic:user p@ssw0rd")]
    public class ControllerApi
    {
        const int MinNumberMotor = 1;
        const int MaxNumberMotor = 5;


        /// <summary>
        /// Initialize all the motors
        /// </summary>
        static public void Initialize()
        {
           
        }

        /// <summary>
        /// Move motor up
        /// </summary>
        /// <param name="e">Web server context</param>
        [Route("mu")]
        public void MotorUp(WebServerEventArgs e)
        {
            var paramsQuery = WebServer.DecodeParam(e.Context.Request.RawUrl);
            var motorNum = GetMotorNumber(paramsQuery);
            if (motorNum < 0)
            {
                WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadRequest);
                return;
            }

            var timing = GetTiming(paramsQuery);
            e.Context.Response.ContentLength64 = 0;
            WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
        }

        /// <summary>
        /// Move motor down
        /// </summary>
        /// <param name="e">Web server context</param>
        [Route("md")]
        public void MotorDown(WebServerEventArgs e)
        {
            var paramsQuery = WebServer.DecodeParam(e.Context.Request.RawUrl);
            var motorNum = GetMotorNumber(paramsQuery);
            if (motorNum < 0)
            {
                WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadRequest);
                return;
            }

            var timing = GetTiming(paramsQuery);
            e.Context.Response.ContentLength64 = 0;
            WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
        }

        /// <summary>
        /// Stop motor
        /// </summary>
        /// <param name="e">Web server context</param>
        [Route("ms")]
        public void MotorStop(WebServerEventArgs e)
        {
            var paramsQuery = WebServer.DecodeParam(e.Context.Request.RawUrl);
            var motorNum = GetMotorNumber(paramsQuery);
            if (motorNum < 0)
            {
                WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadRequest);
                return;
            }

            e.Context.Response.ContentLength64 = 0;
            WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
        }

        /// <summary>
        /// Switch on or off the led
        /// </summary>
        /// <param name="e">Web server context</param>
        [Route("led")]
        public void Led(WebServerEventArgs e)
        {
            var paramsQuery = WebServer.DecodeParam(e.Context.Request.RawUrl);
            if (paramsQuery != null && paramsQuery.Length > 0)
            {
                if (paramsQuery[0].Name == "l")
                {
                    if (paramsQuery[0].Value == "on")
                    {
                        
                    }
                    else
                    {
                       
                    }
                }
            }
        }

        private int GetMotorNumber(UrlParameter[] paramsQuery)
        {
            try
            {
                foreach (var param in paramsQuery)
                {
                    if (param.Name == "p")
                    {
                        var motor = Convert.ToInt32(param.Value);
                        if (motor >= MinNumberMotor && motor <= MaxNumberMotor)
                        {
                            return motor - 1;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return -1;
        }

        private int GetTiming(UrlParameter[] paramsQuery)
        {
            try
            {
                foreach (var param in paramsQuery)
                {
                    if (param.Name == "t")
                    {
                        return Convert.ToInt32(param.Value);
                    }
                }
            }
            catch (Exception)
            {
            }

            return -1;
        }
    }
}
