using B2Net;
using System;

namespace FileExplorer.Models {
	public class Client
	{
		public Guid Id { get; set; }

		public string AppId { get; set; }

		public string AppKey {  get; set; }

		public Client() {
			Id = Guid.NewGuid();	
		}
	}
}
