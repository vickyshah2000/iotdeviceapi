using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjeeviIoT.EntitiesDTO
{
	public class CountryDto
	{
		public int CountryId { get; set; }
		public string? CountryName { get; set; }
		public List<StateDto> States { get; set; }
	}
}