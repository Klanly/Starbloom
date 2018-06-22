using UnityEngine;
namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        /// <summary>
        /// Curve class
        /// </summary>
        [System.Serializable]
        public class BZCurv : System.Object
        {
            public Vector3 m_startPosition;
            public Vector3 m_endPosition;
            public Vector3 m_startTangent;
            public Vector3 m_endTangent;
            public float m_startAngle;
            public float m_endAngle;
            public float m_length = 0;
            public Vector3 m_upRef = Vector3.up;
            public bool legacy_1_5_1 = false;

            Vector3 a;
            Vector3 b;
            Vector3 c;

            [System.Serializable]
            public class Result
            {
                public Vector3 position;
                public Vector3 tangent;
                public float curvPos;
            }


            public BZCurv()
            {

            }

            public BZCurv(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
            {
                m_startPosition = startPosition;
                m_startTangent = startTangent;
                m_endTangent = endTangent;
                m_endPosition = endPosition;
            }

            public Vector3 GetPosition(float t)
            {
                t = Mathf.Clamp01(t);

                c.x = 3f * ((m_startPosition.x + m_startTangent.x) - m_startPosition.x);
                b.x = 3f * ((m_endPosition.x + m_endTangent.x) - (m_startPosition.x + m_startTangent.x)) - c.x;
                a.x = m_endPosition.x - m_startPosition.x - c.x - b.x;

                c.y = 3f * ((m_startPosition.y + m_startTangent.y) - m_startPosition.y);
                b.y = 3f * ((m_endPosition.y + m_endTangent.y) - (m_startPosition.y + m_startTangent.y)) - c.y;
                a.y = m_endPosition.y - m_startPosition.y - c.y - b.y;

                c.z = 3f * ((m_startPosition.z + m_startTangent.z) - m_startPosition.z);
                b.z = 3f * ((m_endPosition.z + m_endTangent.z) - (m_startPosition.z + m_startTangent.z)) - c.z;
                a.z = m_endPosition.z - m_startPosition.z - c.z - b.z;

                float t2 = t * t;
                float t3 = t * t * t;
                float x = a.x * t3 + b.x * t2 + c.x * t + m_startPosition.x;
                float y = a.y * t3 + b.y * t2 + c.y * t + m_startPosition.y;
                float z = a.z * t3 + b.z * t2 + c.z * t + m_startPosition.z;
                return new Vector3(x, y, z);
            }

            public Vector3 GetTangent(float t)
            {
                t = Mathf.Clamp01(t);

                c.x = 3f * ((m_startPosition.x + m_startTangent.x) - m_startPosition.x);
                b.x = 3f * ((m_endPosition.x + m_endTangent.x) - (m_startPosition.x + m_startTangent.x)) - c.x;
                a.x = m_endPosition.x - m_startPosition.x - c.x - b.x;

                c.y = 3f * ((m_startPosition.y + m_startTangent.y) - m_startPosition.y);
                b.y = 3f * ((m_endPosition.y + m_endTangent.y) - (m_startPosition.y + m_startTangent.y)) - c.y;
                a.y = m_endPosition.y - m_startPosition.y - c.y - b.y;

                c.z = 3f * ((m_startPosition.z + m_startTangent.z) - m_startPosition.z);
                b.z = 3f * ((m_endPosition.z + m_endTangent.z) - (m_startPosition.z + m_startTangent.z)) - c.z;
                a.z = m_endPosition.z - m_startPosition.z - c.z - b.z;

                float t2 = t * t;
                float x = 3f * a.x * t2 + 2f * b.x * t + c.x;
                float y = 3f * a.y * t2 + 2f * b.y * t + c.y;
                float z = 3f * a.z * t2 + 2f * b.z * t + c.z;
                return new Vector3(x, y, z).normalized;
            }


            public float GetLength(int stepCount)
            {
                float length = 0;
                for (int i = 0; i < stepCount; ++i)
                {
                    length += (GetPosition((float)i / (float)stepCount) - GetPosition((float)(i + 1) / (float)stepCount)).magnitude;
                }
                return length;
            }



            //public Vector3 GetUp(float t, Vector3 upRef )
            public Vector3 GetUp(float t)
            {
                //return Vector3.Cross(Vector3.Cross(GetTangent(t), m_upRef.normalized).normalized, GetTangent(t));


                //Vector3 tan0 = GetTangent(0);
                //Vector3 up0 = Vector3.Cross(Vector3.Cross(tan0, m_upRef.normalized).normalized, tan0);
                //up0 *= Mathf.Sign(Vector3.Dot(up0, m_upRef));
                //Vector3 r0 = Vector3.Cross(up0, tan0);

                //Vector3 tan1 = GetTangent(1);
                //Vector3 up1 = Vector3.Cross(Vector3.Cross(tan1, m_upRef.normalized).normalized, tan1);
                //up1 *= Mathf.Sign(Vector3.Dot(up1, m_upRef));
                //Vector3 r1 = Vector3.Cross(up1, tan1);

                //if (!legacy_1_5_1)
                {
                    
                    Vector3 right2 = Vector3.Cross((m_endPosition-m_startPosition), m_upRef);
                    //Vector3 upRef = Vector3.Cross(right2, GetTangent(t));
                    //Vector3 right = Vector3.Cross(m_upRef.normalized, tan0) * Mathf.Sign(Vector3.Dot(Vector3.Cross(up0, tan0), Vector3.Cross(m_upRef.normalized, tan0)));
                    //right = Vector3.Cross(m_upRef.normalized, (m_endPosition - m_startPosition).normalized);
                    return -Vector3.Cross(right2, GetTangent(t)).normalized;// * Mathf.Sign( Vector3.Dot( upRef.normalized, m_upRef.normalized) );
                }


				/*

                Vector3 tan = GetTangent(t).normalized;

                Vector3 result2 = Vector3.zero;
                result2 = Vector3.Lerp(up0, up1, t);//smooth result

                Vector3 right = Vector3.Cross(tan, Vector3.Lerp(up0, up1, t)).normalized;
                //result2 = m_upRef;

                result2 = Vector3.Cross(right, tan);

                return result2;


                //up0 = Quaternion.FromToRotation(tan0, tan) * up0;
                //up1 = Quaternion.FromToRotation(tan1, tan) * up1;
                
                
                //tan = Vector3.Lerp(tan0, tan1, t);

                Vector3 r = Vector3.Cross(tan, m_upRef.normalized).normalized;
                if (t > 0f || t < 1f)
                {
                    r = Vector3.Cross(tan, Vector3.Lerp(up0, up1, t)).normalized;
                    //r = Vector3.Cross(GetTangent(Mathf.Clamp01(t - 0.01f)), tan).normalized;
                }

                Vector3 up = Vector3.Cross(r, tan);

                Quaternion q = Quaternion.AngleAxis(Mathf.Lerp(m_startAngle, m_endAngle,t), tan);
                
                Vector3 result = q*Vector3.Normalize(up);

                result += Vector3.Lerp( up0 , up1, t);//smooth result

                return result.normalized;

				*/
            }



            public void InitLength(int stepCount)
            {
                m_length = GetLength(stepCount);
            }


            public Result GetLinearData(float t, int stepCount)
            {
                Result r = new Result();
                float target = m_length * t;
                float tmp = 0;
                //float stepSize = m_length / (float)stepCount;
                for (int i = 0; i < stepCount; ++i)
                {
                    float delta = (GetPosition((float)i / (float)stepCount) - GetPosition((float)(i + 1) / (float)stepCount)).magnitude;
                    if (target > tmp && target < tmp + delta)
                    {
                        r.position = (GetPosition((float)i / (float)stepCount) + GetPosition((float)(i + 1) / (float)stepCount)) / 2f;
                        r.tangent = (GetTangent((float)i / (float)stepCount) + GetTangent((float)(i + 1) / (float)stepCount)) / 2f;
                        r.curvPos = (float)i + .5f;
                        return r;
                    }

                    tmp += delta;
                }
                r.position = GetPosition(1);
                r.tangent = GetTangent(1);
                r.curvPos = 1;
                return r;
            }


            public BZCurv Copy()
            {
                BZCurv c = new BZCurv();
                c.m_startAngle = m_startAngle;                
                c.m_startPosition = m_startPosition;
                c.m_startTangent = m_startTangent;

                c.m_endAngle = m_endAngle;
                c.m_endPosition = m_endPosition;
                c.m_endTangent = m_endTangent;

                c.m_upRef = m_upRef;


                return c;
            }
                       

        }




        /// <summary>
        /// Multiple curve class
        /// </summary>
        [System.Serializable]
        public class MBZCurv : System.Object
        {
            public BZCurv[] m_curves;
            public float m_startAngle;
            public float m_endAngle;

            public Vector3 startPosition
            {
                get { return m_curves[0].m_startPosition; }
                set { m_curves[0].m_startPosition = value; }
            }


            public Vector3 endPosition
            {
                get { return m_curves[m_curves.Length-1].m_endPosition; }
                set { m_curves[m_curves.Length - 1].m_endPosition = value; }
            }

            public Vector3 startTangent
            {
                get { return m_curves[0].m_startTangent; }
                set { m_curves[0].m_startTangent = value; }
            }


            public Vector3 endTangent
            {
                get { return m_curves[m_curves.Length - 1].m_endTangent; }
                set { m_curves[m_curves.Length - 1].m_endTangent = value; }
            }

            public BZCurv GetCurv(int id)
            {
                return m_curves[id];
            }


            public MBZCurv()
            {
                m_curves = new BZCurv[1];
                m_curves[0] = new BZCurv();
            }


            public MBZCurv(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
            {
                m_curves = new BZCurv[1];
                m_curves[0] = new BZCurv(startPosition, endPosition, startTangent, endTangent);
            }

            public MBZCurv( BZCurv c )
            {
                m_curves = new BZCurv[1];
                m_curves[0] = new BZCurv(c.m_startPosition, c.m_endPosition, c.m_startTangent, c.m_endTangent);
            }

            public Vector3 GetPosition(float t)
            {
                int id = Mathf.Clamp((int)(t * m_curves.Length),0, m_curves.Length-1);
                float split = 1 / (float) m_curves.Length;
                t = (t - (split * id)) / split;
                return m_curves[id].GetPosition( t );
            }


            public Vector3 GetUp(float t/*, Vector3 upRef*/)
            {

                int id = Mathf.Clamp((int)(t * m_curves.Length), 0, m_curves.Length - 1);
                float split = 1 / (float)m_curves.Length;
                t = (t - (split * id)) / split;
                Quaternion q = Quaternion.AngleAxis(Mathf.Lerp(m_startAngle, m_endAngle, t), GetTangent(t));

                return q*m_curves[id].GetUp(t);
            }


            public Vector3 GetTangent(float t)
            {
                int id = Mathf.Clamp((int)(t * m_curves.Length), 0, m_curves.Length - 1);
                float split = 1f / (float)m_curves.Length;
                t = (t - (split * id)) / split;
                return m_curves[id].GetTangent(t);
            }

            public float GetLength(int stepCount)
            {
                float l = 0;
                for (int i = 0; i < m_curves.Length; ++i)
                    l += m_curves[i].GetLength(stepCount);
                return l;
            }

            public void Split( int n )
            {
                n = n < 1 ? 1 : n;
                n = n > 10 ? 10 : n;
                BZCurv[] curves = new BZCurv[n];

                for( int i=0; i<n; ++i )
                {
                    float t0 = (float)i / (float)n;
                    float t1 = (float)(i+1) / (float)n;
                    curves[i] = new BZCurv();
                   
                    curves[i].m_startPosition = GetPosition(t0);
                    curves[i].m_endPosition = GetPosition(t1);
                    curves[i].m_startTangent = GetTangent(t0);
                    curves[i].m_endTangent = -GetTangent(t1);

                    if (i > 0)
                        curves[i].m_startTangent *= .01f;
                    else
                        curves[i].m_startTangent = startTangent;

                    if (i < n-1)
                        curves[i].m_endTangent *= .01f;
                    else
                        curves[i].m_endTangent = endTangent;
                }
                m_curves = curves;
            }



            public MBZCurv Copy()
            {
                MBZCurv c = new MBZCurv();
                c.m_curves = new BZCurv[m_curves.Length];
                c.m_startAngle = m_startAngle;
                c.m_endAngle = m_endAngle;
                for ( int i=0; i<m_curves.Length; ++i )
                {
                    c.m_curves[i] = m_curves[i].Copy();
                }
                return c;
            }


            public enum eMirrorAxis
            {
                X,
                Y,
                Z
            }

            public void Mirror(eMirrorAxis axis, Vector3 offset )
            {
                for (int i = 0; i < m_curves.Length; ++i)
                {
                    //m_curves[i].m_startPosition += Vector3.up * .1f;
                    //m_curves[i].m_endPosition -= Vector3.up * .1f;
                    switch (axis)
                    {
                        case eMirrorAxis.X:
                            m_curves[i].m_startPosition.x *= -1f;
                            m_curves[i].m_endPosition.x *= -1f;
                            m_curves[i].m_startTangent.x *= -1f;
                            m_curves[i].m_endTangent.x *= -1f;

                            //m_curves[i].m_startPosition.x -= offset;
                            //m_curves[i].m_endPosition.x -= offset;
                            break;

                        case eMirrorAxis.Y:
                            m_curves[i].m_startPosition.y *= -1f;
                            m_curves[i].m_endPosition.y *= -1f;
                            m_curves[i].m_startTangent.y *= -1f;
                            m_curves[i].m_endTangent.y *= -1f;

                            //m_curves[i].m_startPosition.y -= offset;
                            //m_curves[i].m_endPosition.y -= offset;
                            break;

                        case eMirrorAxis.Z:
                            m_curves[i].m_startPosition.z *= -1f;
                            m_curves[i].m_endPosition.z *= -1f;
                            m_curves[i].m_startTangent.z *= -1f;
                            m_curves[i].m_endTangent.z *= -1f;

                            //m_curves[i].m_startPosition.z -= offset;
                            //m_curves[i].m_endPosition.z -= offset;
                            break;
                    }

                    m_curves[i].m_startPosition -= offset;
                    m_curves[i].m_endPosition -= offset;

                }                
            }


            public void SetUpRef(Vector3 upRef)
            {
                upRef.Normalize();
                for (int i = 0; i < m_curves.Length; ++i)
                {
                    m_curves[i].m_upRef = upRef;
                }
            }


        }

    }
}