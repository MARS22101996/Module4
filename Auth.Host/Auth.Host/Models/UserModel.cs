using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auth.Host.Models
{
	public class UserModel
	{
		public string Email { get; set; }

		public string Name { get; set; }

		public List<string> Role { get; set; }
	}
}