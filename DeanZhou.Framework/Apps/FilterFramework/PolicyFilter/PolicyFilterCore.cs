using System;
using System.Data;

namespace DeanZhou.Framework
{
    public sealed class PolicyFilterCore : FilterBaseCore<DataRowContainer, DataRowContainer, DataRow, NeedPolicyType>
    {

        public float MaxDecreasePoint { get; set; }

        public float MaxRateDiscount { get; set; }

        public float CurrRateDiscount { get; set; }

        //构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="maxDecreasePoint">返点下降的最大百分比</param>
        /// <param name="identifyItemTypeAsEnumType">政策类型识别器</param>
        /// <param name="getStorItemType"></param>
        public PolicyFilterCore(float maxDecreasePoint, Func<DataRowContainer, DataRowContainer, NeedPolicyType> identifyItemTypeAsEnumType, Func<DataRowContainer, DataRowContainer, DataRow> getStorItemType)
            : base(identifyItemTypeAsEnumType, getStorItemType)
        {
            MaxRateDiscount = 0;
            MaxDecreasePoint = maxDecreasePoint;
        }

        //数据初始化
        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="currNeedType">当前需要的政策类型</param>
        /// <param name="currEachGetCount">每种类型需要获取的最小个数</param>
        /// <param name="maxCheckCount">同航线最多检测的政策条数</param>
        public override void Init(NeedPolicyType currNeedType, int currEachGetCount, int maxCheckCount)
        {
            base.Init(currNeedType, currEachGetCount, maxCheckCount);
            SetItemAddedCompleted(trc =>
            {
                if (MaxRateDiscount <= 0)
                {
                    MaxRateDiscount = trc.Float("Discount");
                }
                CurrRateDiscount = trc.Float("Discount");
            });
            SetCustomerCheckFinished(x =>
            {
                PolicyFilterCore pfc = x as PolicyFilterCore;
                if (pfc == null)
                {
                    return false;
                }
                bool isDiscountFinished = pfc.MaxRateDiscount * (1 - pfc.MaxDecreasePoint) > pfc.CurrRateDiscount;
                if (isDiscountFinished)
                {
                    return true;
                }
                string airlineCode = pfc.CurrItem.String("AircomE").Substring(0, 2);
                foreach (DataRow pickedPolicy in pfc.CurrPickedData.Keys)
                {
                    string airlineCode1 = pickedPolicy["AircomE"].ToString().Substring(0, 2);
                    bool isOut = airlineCode != airlineCode1;
                    if (isOut)
                    {
                        return true;
                    }
                }
                return false;
            });
        }
    }

}
