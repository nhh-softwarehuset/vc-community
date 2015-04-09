﻿using System;
using VirtoCommerce.Domain.Marketing.Model;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.MarketingModule.Expressions.Promotion
{
	//[] [] items of entry are in shopping cart
	public class ConditionAtNumItemsOfEntryAreInCart : ConditionBase, IConditionExpression
	{
		public decimal NumItem { get; set; }
		public bool Exactly { get; set; }
		public string SelectedItemId { get; set; }

		#region IConditionExpression Members
		/// <summary>
		/// ((PromotionEvaluationContext)x).GetCartItemsOfProductQuantity(SelectedItemId, ExcludingCategoryIds, ExcludingProductIds) > NumItem
		/// </summary>
		/// <returns></returns>
		public linq.Expression<Func<IPromotionEvaluationContext, bool>> GetConditionExpression()
		{
			var paramX = linq.Expression.Parameter(typeof(IPromotionEvaluationContext), "x");
			var castOp = linq.Expression.MakeUnary(linq.ExpressionType.Convert, paramX, typeof(PromotionEvaluationContext));
			var methodInfo = typeof(PromotionEvaluationContextExtension).GetMethod("GetCartItemsOfProductQuantity");
			var methodCall = linq.Expression.Call(null, methodInfo, castOp, linq.Expression.Constant(SelectedItemId));
			var numItem = linq.Expression.Constant(NumItem);
			var binaryOp = Exactly ? linq.Expression.Equal(methodCall, numItem) : linq.Expression.GreaterThanOrEqual(methodCall, numItem);

			var retVal = linq.Expression.Lambda<Func<IPromotionEvaluationContext, bool>>(binaryOp, paramX);

			return retVal;
		}

		#endregion
	}
}
