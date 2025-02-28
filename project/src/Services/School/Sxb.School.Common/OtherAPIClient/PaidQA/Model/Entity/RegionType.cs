using System;
using Kogel.Dapper.Extension.Attributes;

namespace Sxb.School.Common.OtherAPIClient.PaidQA.Model.Entity
{
	[Serializable]
	public class RegionType
	{
		 /// <summary> 
		 /// </summary> 
		[Identity]
		public Guid ID {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Name {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? PID {get;set;}

		 /// <summary> 
		 /// </summary> 
		public int? Sort {get;set;}

		 /// <summary> 
		 /// </summary> 
		public bool? IsValid {get;set;}
	}
}