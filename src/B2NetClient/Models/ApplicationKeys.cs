using B2Net;
using System;

namespace FileExplorer.Models {
	public class ApplicationKeys
	{
		public Guid Id { get; set; }

		public string AppId { get; set; }

		public string AppKey {  get; set; }

		public ApplicationKeys() {
			Id = Guid.NewGuid();	
		}
	}
}
