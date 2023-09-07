using MagicVilla_Web.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.ViewModels
{
    public class VillaNumberDeleteVM
    {
        public VillaNumberDTO VillaNumber { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? VillaList { get; set; }

        public VillaNumberDeleteVM()
        {
            VillaNumber = new VillaNumberDTO();
        }
    }
}
