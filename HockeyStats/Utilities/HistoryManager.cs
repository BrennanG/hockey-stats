using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public class HistoryManager<TValue>
    {
        private Stack<TValue> backStack = new Stack<TValue>();
        private Stack<TValue> forwardStack = new Stack<TValue>();
        private TValue currentItem;

        public HistoryManager()
        {
        }

        public void AddItem(TValue item)
        {
            if (item == null) { throw new Exception("Cannot add a null value."); }
            if (currentItem != null)
            {
                backStack.Push(currentItem);
            }
            currentItem = item;
            forwardStack.Clear();
        }

        public TValue GetCurrentItem()
        {
            return currentItem;
        }

        public void UpdateCurrentItem(TValue updatedItem)
        {
            currentItem = updatedItem;
        }

        public TValue MoveToNextItem()
        {
            if (forwardStack.Count < 1) { throw new Exception("There is no next item."); }
            backStack.Push(currentItem);
            currentItem = forwardStack.Pop();
            return currentItem;
        }

        public TValue MoveToPreviousItem()
        {
            if (backStack.Count < 1) { throw new Exception("There is no previous item."); }
            forwardStack.Push(currentItem);
            currentItem = backStack.Pop();
            return currentItem;
        }

        public bool CanMoveForward()
        {
            return forwardStack.Count > 0;
        }

        public bool CanMoveBack()
        {
            return backStack.Count > 0;
        }
    }
}
