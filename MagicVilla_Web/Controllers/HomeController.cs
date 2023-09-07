using Microsoft.AspNetCore.Mvc;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using MagicVilla_Web.DTOs;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers;

public class HomeController : Controller
{

    private readonly IVillaService _villaService;

    public HomeController(IVillaService villaService)
    {
        _villaService = villaService;
    }

    public async Task<IActionResult> Index()
    {
        List<VillaDTO> list = new();

        var response = await _villaService.GetAllAsync<APIResponse>();
        if (response is not null && response.IsSucess)
        {
            list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
        }

        return View(list);
    }
}
