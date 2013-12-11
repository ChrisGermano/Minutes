
using System;
using System.Collections.Generic;

namespace MinutesToMidnight
{
	public class Hasher
	{
        private static Hasher m_instance;
        private Dictionary<int, System.String> m_masterHashMap;
		
        private Hasher ()
		{
		}

        public static Hasher Instance
        {
            get
            {
                if(m_instance == null)
                {
                    m_instance = new Hasher();
                }
                return m_instance;
            }
        }
        /*
        public bool hash(System.String hashstring, out int hashkeys)
        {
            return true;
        }
        */
	}
}