using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AOGweb.Data;
using AOGweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AOGweb.Controllers
{
    public class ArduinoController : Controller
    {
        [HttpPut]
        public async Task<IActionResult> Set1([FromBody] Device1 arduino)
        {
            using var _context = new WebAppContext();
            var device = await _context.Devices1.FindAsync(arduino.MAC);
            device.LastUpdate = DateTime.Now;
            device.ERROR = arduino.ERROR;
            device.Temperature = arduino.Temperature;
            device.Humidity = arduino.Humidity;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500); //Internal server error
            }

            return Ok();
        }

        //if switched from web, update device
        [HttpPost]
        public async Task<IActionResult> Get2([FromBody] Device2 arduino)
        {
            using var _context = new WebAppContext();
            var device = await _context.Devices2.FindAsync(arduino.MAC);
            device.LastUpdate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500); //Internal server error
            }

            return Ok(new { device.State1, device.State2 });
        }

        //if manually switch then update
        [HttpPut]
        public async Task<IActionResult> Set2([FromBody] Device2 arduino)
        {
            using var _context = new WebAppContext();
            var device = await _context.Devices2.FindAsync(arduino.MAC);
            device.LastUpdate = DateTime.Now;
            device.State1 = arduino.State1;
            device.State2 = arduino.State2;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500); //Internal server error
            }
            return Ok();
        }
    }
}