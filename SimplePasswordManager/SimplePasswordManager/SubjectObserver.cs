using System.Collections.Generic;

namespace SimplePasswordManager
{
    public interface IObserver
    {
        void OnNotify(object s, string e, object p);
    }

    public class Subject 
    {
        public void AddObserver(IObserver observer)
        {
            m_observers.Add(observer);
            OnAdd(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            m_observers.Remove(observer);
            OnRemove(observer);
        }

        public virtual void OnAdd(IObserver observer) {}
        public virtual void OnRemove(IObserver observer) {}

        protected void NotifyObservers(string e, object p)
        {
            foreach(var observer in m_observers)
            {
                observer.OnNotify(this, e, p);
            }
        }

        private List<IObserver> m_observers = new List<IObserver>();
    }
}
