using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Anima2D 
{
	public class Bone2D : MonoBehaviour
	{
		[SerializeField][FormerlySerializedAs("color")]
		Color m_Color = Color.white;
        
        [SerializeField]
        float m_minRotationConstraint = 0f;
        
        [SerializeField]
        float m_maxRotationConstraint = 0f;
        

		[SerializeField][FormerlySerializedAs("mLength")]
		float m_Length = 1f;

		//DEPRECATED
		[SerializeField][HideInInspector][FormerlySerializedAs("mChild")]
		Bone2D m_Child;

		[SerializeField]
		bool debug = false;

		[SerializeField][HideInInspector]
		Transform m_ChildTransform;

		[SerializeField][HideInInspector]
		public Transform[] m_ControlTransform;

		[SerializeField]
		Ik2D m_AttachedIK;
		public Ik2D attachedIK
		{
			get { return m_AttachedIK; }
			set { m_AttachedIK = value; }
		}

		public Color color {
			get {
				return m_Color;
			}
			set {
				m_Color = value;
			}
		}
        
        public float minRotationConstraint {
            get {
                return m_minRotationConstraint;
            }    
            set {
                m_minRotationConstraint = value;
            }        
        }
        
        public float maxRotationConstraint {
            get {
                return m_maxRotationConstraint;
            }    
            set {
                m_maxRotationConstraint = value;
            }        
        }

		Bone2D m_CachedChild;


		public Bone2D child
		{
			get {
				if(m_Child)
				{
					child = m_Child;
				}

				if(m_CachedChild && m_ChildTransform != m_CachedChild.transform)
				{
					m_CachedChild = null;
				}

				if(m_ChildTransform && m_ChildTransform.parent != transform)
				{
					m_CachedChild = null;
				}

				if(!m_CachedChild && m_ChildTransform && m_ChildTransform.parent == transform)
				{
					m_CachedChild = m_ChildTransform.GetComponent<Bone2D>();
				}

				return m_CachedChild;
			}

			set {
				m_Child = null;
				m_CachedChild = value;
				m_ChildTransform = m_CachedChild.transform;
			}
		}

		public Vector3 localEndPosition
		{
			get {
				return Vector3.right*localLength;
			}
		}

		public Vector3 endPosition
		{
			get {
				return transform.TransformPoint(localEndPosition);
			}
		}

		public float localLength {
			get {
				if(child)
				{
					Vector3 childPosition = transform.InverseTransformPoint(child.transform.position);
					m_Length = Mathf.Clamp(childPosition.x,0f,childPosition.x);
				}

				return m_Length;
			}
			set {
				if(!child)
				{
					m_Length = value;
				}
			}
		}

		public float length {
			get {
				return transform.TransformVector(localEndPosition).magnitude;
			}
		}

		Bone2D mParentBone = null;
		public Bone2D parentBone
		{
			get {
				Transform parentTransform = transform.parent;

				if(!mParentBone)
				{
					if(parentTransform)
					{
						mParentBone = parentTransform.GetComponent<Bone2D>();
					}
				}else if(parentTransform != mParentBone.transform)
				{
					if(parentTransform)
					{
						mParentBone = parentTransform.GetComponent<Bone2D>();
					}else{
						mParentBone = null;
					}
				}
				
				return mParentBone;
			}
		}

		public Bone2D linkedParentBone
		{
			get {
				if(parentBone && parentBone.child == this)
				{
					return parentBone;
				}
				
				return null;
			}
		}
		
		public Bone2D root
		{
			get {
				Bone2D rootBone = this;
				
				while(rootBone.parentBone)
				{
					rootBone = rootBone.parentBone;
				}
				
				return rootBone;
			}
		}

		public Bone2D chainRoot
		{
			get {
				Bone2D chainRoot = this;
				
				while(chainRoot.parentBone && chainRoot.parentBone.child == chainRoot)
				{
					chainRoot = chainRoot.parentBone;
				}
				
				return chainRoot;
			}
		}

		public int chainLength
		{
			get {
				Bone2D chainRoot = this;

				int length = 1;

				while(chainRoot.parentBone && chainRoot.parentBone.child == chainRoot)
				{
					++length;
					chainRoot = chainRoot.parentBone;
				}
				
				return length;
			}
		}

		public static Bone2D GetChainBoneByIndex(Bone2D chainTip, int index)
		{
			if(!chainTip) return null;
			
			Bone2D bone = chainTip;
			
			int chainLength = bone.chainLength;
			
			for(int i = 0; i < chainLength && bone; ++i)
			{
				if(i == index)
				{
					return bone;
				}
				
				if(bone.linkedParentBone)
				{
					bone = bone.parentBone;
				}else{
					return null;
				}
			}
			
			return null;
		}
        
		private float m_minConstraintLessThan = 0;
		private float m_minConstraintGreaterThan = 0;
		private float m_maxConstraintLessThan = 0;
		private float m_maxConstraintGreaterThan = 0;
		private bool m_hasConstraints = true;
		[ExecuteInEditMode]

		private Control[] m_control;

		public Control[] controls
		{
			get {
				if (m_ControlTransform == null || m_ControlTransform.Length == 0){
					return null;
				}
				if (m_control == null || m_ControlTransform.Length != m_control.Length)
				{
					List<Control> c = new List<Control>(); 
					foreach (var item in m_ControlTransform)
					{
						c.Add(item.GetComponent<Control>());
					}
					m_control = c.ToArray();
				}				
				return m_control;
			}

		}
		void Start(){
			
			m_hasConstraints = m_minRotationConstraint != 0 || m_maxRotationConstraint != 0;
			m_maxRotationConstraint %= 360f;
			m_minRotationConstraint %= 360f;
			if (m_minRotationConstraint > m_maxRotationConstraint){
				float temp = m_maxRotationConstraint;
				m_maxRotationConstraint = m_minRotationConstraint;
				m_minRotationConstraint = temp;
			}
			if (m_minRotationConstraint < 0 && m_maxRotationConstraint < 0){
				m_minRotationConstraint += 360f;
				m_maxRotationConstraint += 360f;
			}
			float diff = m_maxRotationConstraint - m_minRotationConstraint;
			float excludedDiff = 360f - diff;
			if (excludedDiff <= 0 || (m_maxRotationConstraint == 0 && m_minRotationConstraint == 0)){
				m_hasConstraints = false;
			} else {
				if (m_minRotationConstraint < 0){
					m_minConstraintGreaterThan = m_minRotationConstraint + 360f;
					m_minConstraintLessThan = 360f + (m_minConstraintGreaterThan + (excludedDiff / 2f));
					m_minConstraintLessThan %= 360f;
					// USE AND
				} else {
					m_minConstraintLessThan = m_minRotationConstraint;
					m_minConstraintGreaterThan = 360f + (m_minConstraintLessThan - (excludedDiff / 2f));
					m_minConstraintGreaterThan %= 360f;
					// USE OR
				}
				if (m_maxRotationConstraint < 0){
					m_maxConstraintLessThan = m_maxRotationConstraint + 360f;
					m_maxConstraintGreaterThan = 360f + (m_maxConstraintLessThan - (excludedDiff / 2f));
					m_maxConstraintGreaterThan %= 360f;
					// USE AND
				} else {					
					m_maxConstraintGreaterThan = m_maxRotationConstraint;
					m_maxConstraintLessThan = 360f + (m_maxRotationConstraint + (excludedDiff / 2f));
					m_maxConstraintLessThan %= 360f;
					// USE OR
				}
			}
		}
        public void SetLocalRotation(Quaternion rotation){
            
            if (m_hasConstraints){
                Vector3 rot = rotation.eulerAngles;
				float z = rot.z % 360f;
				if (z < m_minRotationConstraint || z > m_maxRotationConstraint){
					
					if (m_minConstraintLessThan < m_minConstraintGreaterThan){
						// use OR
						if (z < m_minConstraintLessThan || z > m_minConstraintGreaterThan){
							z = m_minRotationConstraint;
						}
					} else {
						// use OR
						if (z < m_minConstraintLessThan && z > m_minConstraintGreaterThan){
							z = m_minRotationConstraint;
						}
					}
					if (m_maxConstraintLessThan < m_maxConstraintGreaterThan){
						// use OR
						if (z < m_maxConstraintLessThan || z > m_maxConstraintGreaterThan){
							z = m_maxRotationConstraint;
						}
					} else {
						// use AND
						if (z < m_maxConstraintLessThan && z > m_maxConstraintGreaterThan){
							z = m_maxRotationConstraint;
						}
					}
					if (z < 0){
						z += 360f;
					}                
				}				
                transform.localEulerAngles = new Vector3(rot.x, rot.y, z);				                                
            } else {
				transform.localRotation = rotation;				
			}
			if (debug){
				
			} 
			Control[] controls = this.controls;
			if (controls != null){
				foreach (var item in controls)
				{
					item.UpdateControlFromBone(this);	
				}				
			}
            
        }
	}
}
