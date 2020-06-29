using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AOGweb.Data;
using AOGweb.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace AOGweb.Controllers
{
    [Authorize]
    public class DevicesController : Controller
    {
        public async Task<IActionResult> Index(string room = "ALL")
        {
            using var _context = new WebAppContext();
            var devicesList = await _context.UserDevices.Where(ud => ud.UserID == int.Parse(User.Identity.Name)).ToListAsync();
            TempData["userDevices"] = devicesList;
            var devicesMACList = devicesList.Select(ud => ud.DeviceMAC).ToList();
            var devices = await _context.Devices.Where(d => devicesMACList.Contains(d.MAC)).ToListAsync();
            devices = devices.OrderByDescending(d => d.SettingTime).ToList();
            var rooms = devices.Select(d => d.Room).Distinct().ToList();
            TempData["rooms"] = rooms;
            TempData["selectedRoom"] = room;
            if (room == "ALL")
                return View(devices);
            else
                return View(devices.Where(d => d.Room == room));
        }

        [HttpGet]
        public async Task<IActionResult> Show(string name)
        {
            using var _context = new WebAppContext();
            var devicesMACList = await _context.UserDevices.Where(ud => ud.UserID == int.Parse(User.Identity.Name) && ud.OwnerID == int.Parse(User.Identity.Name)).Select(ud => ud.DeviceMAC).ToListAsync();
            var devices = await _context.Devices.Where(d => devicesMACList.Contains(d.MAC)).ToListAsync();
            var dev = devices.SingleOrDefault(d => d.Name == name);
            if (dev != null)
            {
                string MAC = dev.MAC;
                TempData["rooms"] = devices.Select(d => d.Room).Distinct().ToList();

                var deviceShares = from d in _context.Users
                                   join ud in _context.UserDevices on d.ID equals ud.UserID
                                   where ud.UserID != int.Parse(User.Identity.Name) && ud.OwnerID == int.Parse(User.Identity.Name) && ud.DeviceMAC == MAC
                                   select d.Email;
                TempData["shares"] = deviceShares.ToList();
                var device = devices.Single(d => d.Name == name);
                return View(device);
            }
            else
                return StatusCode(204); //do nothing
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<int> UpdateName(string deviceID, string name)
        {
            using var _context = new WebAppContext();

            var devicesList = await _context.UserDevices.Where(ud => ud.UserID == int.Parse(User.Identity.Name)).Select(ud => ud.DeviceMAC).ToListAsync();
            var devices = await _context.Devices.Where(d => devicesList.Contains(d.MAC)).ToListAsync();
            if (devices.Select(d => d.Name).ToList().Contains(name))
            {
                return -2; //name not unique
            }
            else
            {
                var device = devices.Single(d => d.MAC == deviceID);
                device.Name = name;

                try
                {
                    await _context.SaveChangesAsync();
                    return 0; //name updated
                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine(ex.Message);
                    return -3; //db error
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<int> AddDevice(string deviceID, string name, string room)
        {
            using var _context = new WebAppContext();

            var device = await _context.Devices.SingleOrDefaultAsync(d => d.MAC == deviceID);
            if (device == null)
            {
                return -1; //invalid ID
            }

            var userDevice = await _context.UserDevices.SingleOrDefaultAsync(ud => ud.DeviceMAC == deviceID);
            if (userDevice != null)
            {
                if (userDevice.OwnerID == int.Parse(User.Identity.Name))
                    return 0; //already added
                else
                    return -4;
            }

            var devicesList = await _context.UserDevices.Where(ud => ud.UserID == int.Parse(User.Identity.Name)).Select(ud => ud.DeviceMAC).ToListAsync();
            var devicesName = await _context.Devices.Where(d => devicesList.Contains(d.MAC)).Select(d => d.Name).ToListAsync();
            if (devicesName.Contains(name))
            {
                return -2; //name not unique
            }

            _context.UserDevices.Add(new UserDevices()
            {
                UserID = int.Parse(User.Identity.Name),
                DeviceMAC = deviceID,
                OwnerID = int.Parse(User.Identity.Name)
            });

            device.SettingTime = DateTime.Now;
            device.Name = name;
            device.Room = room;

            try
            {
                await _context.SaveChangesAsync();
                return 0; //added
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine(ex.Message);
                return -3; //db error
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<int> RemoveDevice(string deviceID)
        {
            using var _context = new WebAppContext();

            var device = await _context.Devices.SingleOrDefaultAsync(d => d.MAC == deviceID);
            if (device == null)
            {
                return -1; //invalid ID
            }

            var entries = await _context.UserDevices.Where(ud => ud.OwnerID == int.Parse(User.Identity.Name) && ud.DeviceMAC == deviceID).Select(ud => ud).ToListAsync();
            if (entries.Count > 0)
            {
                _context.UserDevices.RemoveRange(entries);
                device.Name = null;
                device.Room = null;
                device.LastUpdate = device.SettingTime = DateTime.Parse("2020-01-01 00:00:00.000");

                try
                {
                    await _context.SaveChangesAsync();
                    return 0; //removed
                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine(ex.Message);
                    return -3; //db error
                }
            }
            else
                return -2; //invalid User Device
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async void UpdateLight1(string MAC, bool state1)
        {
            Debug.WriteLine($"{state1}");
            using var _context = new WebAppContext();
            var myDevice = await _context.UserDevices.SingleOrDefaultAsync(ud => ud.UserID == int.Parse(User.Identity.Name) && ud.DeviceMAC == MAC);
            if (myDevice != null)
            {
                var device = await _context.Devices2.SingleOrDefaultAsync(d => d.MAC == MAC);
                device.State1 = state1;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) { }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async void UpdateLight2(string MAC, bool state2)
        {
            Debug.WriteLine($"{state2}");
            using var _context = new WebAppContext();
            var myDevice = await _context.UserDevices.SingleOrDefaultAsync(ud => ud.UserID == int.Parse(User.Identity.Name) && ud.DeviceMAC == MAC);
            if (myDevice != null)
            {
                var device = await _context.Devices2.SingleOrDefaultAsync(d => d.MAC == MAC);
                device.State2 = state2;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) { }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<int> ShareDevice(string deviceID, string email)
        {
            using var _context = new WebAppContext();

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return -1; //email do not exist
            }

            var share = await _context.UserDevices.SingleOrDefaultAsync(ud => ud.DeviceMAC == deviceID && ud.OwnerID == int.Parse(User.Identity.Name) && ud.UserID == user.ID);
            if(share != null)
            {
                return -2; //already shared with this email
            }

            _context.UserDevices.Add(new UserDevices()
            {
                UserID = user.ID,
                DeviceMAC = deviceID,
                OwnerID = int.Parse(User.Identity.Name)
            });

            try
            {
                await _context.SaveChangesAsync();
                return 0; //shared
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine(ex.Message);
                return -3; //db error
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<int> RemoveShare(string deviceID, string email)
        {
            using var _context = new WebAppContext();

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            var share = await _context.UserDevices.SingleAsync(ud => ud.DeviceMAC == deviceID && ud.OwnerID == int.Parse(User.Identity.Name) && ud.UserID == user.ID);
            _context.UserDevices.Remove(share);

            try
            {
                await _context.SaveChangesAsync();
                return 0; //shared
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine(ex.Message);
                return -3; //db error
            }
        }
    }
}
