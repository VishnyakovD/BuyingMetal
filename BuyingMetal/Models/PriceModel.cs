using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuyingMetal.Models
{
	public class PriceModel
	{
		public PriceItemModel Block1 { set; get; }
		public PriceItemModel Block2 { set; get; }
		public PriceItemModel Block3 { set; get; }
		public List<PriceItemModel> ListPriceItem { set; get; }

		public PriceModel()
		{
			this.Block1 = new PriceItemModel();
			this.Block2 = new PriceItemModel();
			this.Block3 = new PriceItemModel();
			this.ListPriceItem = new List<PriceItemModel>();
		}
	}

	public class PriceItemModel
	{
		public int Id { set; get; }
		public string Name { set; get; }
		public string Price { set; get; }
		public string Description { set; get; }

		public PriceItemModel()
		{
			this.Name = this.Price = this.Description = string.Empty;
		}
	}
}