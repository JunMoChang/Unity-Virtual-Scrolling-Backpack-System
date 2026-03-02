using System;
using System.Collections.Generic;

namespace Model
{
    public class HotBarModel
    {
        private readonly ItemDataModel[] items;
        public HashSet<ItemDataModel> DataSet { get; }
        private readonly int slotCount = 8;
        
        public event Action<int, ItemDataModel> OnSlotChanged; 
        
        public HotBarModel()
        {
            items = new ItemDataModel[slotCount];
            DataSet = new HashSet<ItemDataModel>();
        }
        
        /// <summary>
        /// 添加物品到快捷栏
        /// </summary>
        /// <param name="itemDataModel">添加的物品</param>
        /// <param name="slotIndex">添加到快捷栏的索引</param>
        /// <returns></returns>
        public bool AddToHotBar(ItemDataModel itemDataModel, int slotIndex)
        {
            if(!IsValidIndex(slotIndex)) return false;
        
            if(itemDataModel == null || itemDataModel.IsEmpty()) return false;
        
            int existingSlotIndex = FindDataIndex(itemDataModel);
            if (existingSlotIndex >= 0 && existingSlotIndex != slotIndex)
            {
                RemoveData(existingSlotIndex);
            }
            else
            {
                DataSet.Add(itemDataModel);
            }
        
            items[slotIndex] = itemDataModel;
        
            NotifySlotChanged(slotIndex);
            return true;
        }
    
        /// <summary>
        /// 交换两个快捷栏的数据
        /// </summary>
        /// <param name="slotIndex1">原快捷栏索引</param>
        /// <param name="slotIndex2">目标快捷栏索引</param>
        public bool SwapSlots(int slotIndex1, int slotIndex2)
        {
            if (!IsValidIndex(slotIndex1) || !IsValidIndex(slotIndex2)) return  false;
        
            (items[slotIndex1], items[slotIndex2]) = (items[slotIndex2], items[slotIndex1]);

            NotifySlotChanged(slotIndex1);
            NotifySlotChanged(slotIndex2);
            
            return true;
        }

        public  ItemDataModel[] GetDataModels()
        {
            return items;
        }
        
        /// <summary>
        /// 查找物品所在的快捷栏位索引
        /// </summary>
        /// <returns>索引，如果不存在返回-1</returns>
        private int FindDataIndex(ItemDataModel dataModel)
        {
            if (dataModel == null) return -1;
        
            for (int i = 0; i < slotCount; i++)
            {
                if (items[i] == dataModel)
                {
                    return i;
                }
            }
            
            return -1;
        }
        
        /// <summary>
        /// 清空指定快捷栏位
        /// </summary>
        public bool RemoveData(int slotIndex)
        {
            if (!IsValidIndex(slotIndex)) return false;
        
            if (items[slotIndex] != null)
            {
                DataSet.Remove(items[slotIndex]);
                items[slotIndex] = null;
                
                NotifySlotChanged(slotIndex);
            }
            return true;
        }

        private bool IsValidIndex(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < slotCount;
        }
        
        private void NotifySlotChanged(int slotIndex)
        {
            OnSlotChanged?.Invoke(slotIndex, items[slotIndex]);
        }
    }
}