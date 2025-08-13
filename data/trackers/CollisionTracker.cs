using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CAC.data.trackers
{
    public class CollisionTracker
    {

        // Ayarlar
        LayerMask groundMask;
        float capsuleRadius = 0.25f;     // karakter yarıçapı
        float capsuleHeight = 1.8f;      // karakter boyu
        float skin = 0.05f;              // tolerans
        float maxGroundAngle = 60f;      // kabul edilen max eğim
        float extraProbe = 0.1f;         // yere “yakın” kabul mesafesi

        public bool grounded { get; private set; }
        public Vector3 groundNormal { get; private set; } = Vector3.up;

        // Tick’te çağır
        public void UpdateGrounded(Vector3 serverPos)
        {
            // Kapsül uçlarını hesapla (dikey kapsül)
            float half = (capsuleHeight * 0.5f) - capsuleRadius;
            Vector3 center = serverPos;
            Vector3 top = center + Vector3.up * half;
            Vector3 bottom = center - Vector3.up * half;

            // Aşağı doğru kısa bir CapsuleCast (ayakların hemen altını yokluyoruz)
            float castDist = skin + extraProbe;
            if (Physics.CapsuleCast(top, bottom, capsuleRadius, Vector3.down, out RaycastHit hit, castDist, groundMask, QueryTriggerInteraction.Ignore))
            {
                // Zemin açısı kontrolü
                groundNormal = hit.normal;
                float angle = Vector3.Angle(groundNormal, Vector3.up);
                bool slopeOk = angle <= maxGroundAngle;

                // “Gerçekten temas” – kapsülün altıyla yüzey arası çok küçükse yere basıyor say
                bool touchClose = hit.distance <= (skin + 0.005f);

                grounded = slopeOk && (touchClose || IsPenetratingGround(top, bottom));
            }
            else
            {
                // Cast boşsa, yine de kapsül zemine gömülü mü diye test et (kenar/iniş anı)
                grounded = IsPenetratingGround(top, bottom);
                groundNormal = Vector3.up;
            }
        }

        // Kapsül yüzeyle kesişiyorsa (tam oturmuş anlarda) penetrasyon verisiyle teyit
        private bool IsPenetratingGround(Vector3 top, Vector3 bottom)
        {
            // Kapsülün yaklaşık merkezini al
            Vector3 approxCenter = (top + bottom) * 0.5f;
            if (Physics.OverlapCapsuleNonAlloc(top, bottom, capsuleRadius, _overlapBuf, groundMask, QueryTriggerInteraction.Ignore) > 0)
            {
                foreach (var col in _overlapBuf)
                {
                    if (col == null) continue;
                    if (Physics.ComputePenetration(
                        col, col.bounds.center, col.transform.rotation,
                        _capsuleProxy, approxCenter, Quaternion.identity,
                        out Vector3 dir, out float dist))
                    {
                        // Aşağı yönlü bir ayrışma ve küçük mesafe => yerde say
                        if (Vector3.Dot(dir, Vector3.up) > 0.25f && dist > 0.001f)
                            return true;
                    }
                }
            }
            return false;
        }

        // Basit bir kapsül proxy’si (Gizmo/penetration için)
        CapsuleCollider _capsuleProxy;
        Collider[] _overlapBuf = new Collider[8];

        public void init()
        {
            // Sahte bir kapsül kolider oluşturup sadece hesaplamalarda kullanıyoruz (render/physics'e eklemiyoruz)
            var go = new GameObject("CapsuleProxy");
            go.hideFlags = HideFlags.HideAndDontSave;
            _capsuleProxy = go.AddComponent<CapsuleCollider>();
            _capsuleProxy.direction = 1; // Y ekseni
            _capsuleProxy.radius = capsuleRadius;
            _capsuleProxy.height = capsuleHeight;
            _capsuleProxy.enabled = false; // world’a etki etmesin
        }

    }
}
