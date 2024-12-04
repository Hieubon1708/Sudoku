using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Soduku
{
    public class PageSwip : MonoBehaviour
    {
        public Scrollbar scrollbar;
        float scroll_pos = 0;
        float[] pos;
        private void Update()
        {
            pos=new float[transform.childCount];
            float distace = 1f / (pos.Length - 1f);
            for(int i=0;i<pos.Length;i++)
            {
                pos[i] = distace * i;
            }
           
            if (Input.GetMouseButton(0))
            {
                if(scrollbar.value>=0 && scrollbar.value <= 1)
                {
                    scroll_pos = scrollbar.value;
                }              
            }
            else
            {
                for (int i = 0; i < pos.Length; i++)
                {
                    if (scroll_pos < pos[i] + (distace / 2) && scroll_pos > pos[i] - distace / 2)
                    {
                        if (Mathf.Lerp(scrollbar.value, pos[i], 0.1f)>=0 && Mathf.Lerp(scrollbar.value, pos[i], 0.1f)<=1)
                         {
                            scrollbar.value = Mathf.Lerp(scrollbar.value, pos[i], 0.1f);
                        }
                       
                    }
                }
            }
        }
    }

}