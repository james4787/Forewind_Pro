using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Forewind
{
	public class CloseAnimation : MonoBehaviour {

        private Animator ator;

        private void Awake()
        {
            ator = this.GetComponent<Animator>();
        }

        public void OnClickedToClose()
        {
            //Debug.LogFormat("--- has ator1 && ator2, animIndex:{0}", mAnimIndex % mAnimTotal);
            ator.Play("Close"); //设置状态控制参数值，用来切状态
        }
	}
}