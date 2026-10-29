using System.Collections.Generic;
using UnityEngine;

namespace Kaveh
{
    /// <summary>
    /// اسکلت قطعه‌قطعه کاوه و انیمیشن‌های پایه (ایستادن، راه رفتن، دراز کردن دست).
    /// مستقل از مرحله است؛ هر مرحله فقط KavehVisual.Tick را صدا می‌زند.
    /// </summary>
    public class KavehRig
    {
        readonly Dictionary<string, Transform> nodes = new Dictionary<string, Transform>();
        Transform torso;
        float ppu = 400f;
        float baseHipY;
        float phase, walkWeight, clock;

        public float Scale { get; private set; }
        public Transform Backpack { get { Transform t; return nodes.TryGetValue("backpack", out t) ? t : torso; } }

        // پارامترهای چرخه راه رفتن (درجه)
        const float ThighSwing = 24f, KneeBend = 42f, ArmSwing = 20f, Lean = -4f;

        public static KavehRig Build(Transform root, float visualHeight)
        {
            var sprites = new Dictionary<string, Sprite>();
            foreach (var p in KavehRigData.Parts)
            {
                var s = Resources.Load<Sprite>("Kaveh/Rig/" + p.name);
                if (s == null) { Debug.LogWarning("KAVEH rig: missing sprite " + p.name); return null; }
                sprites[p.name] = s;
            }

            var rig = new KavehRig();
            rig.ppu = sprites["torso"].pixelsPerUnit;
            rig.baseHipY = KavehRigData.HipToGroundPx / rig.ppu;
            rig.Scale = visualHeight / (KavehRigData.StandingHeightPx / rig.ppu);

            var defs = new Dictionary<string, KavehRigData.Part>();
            foreach (var p in KavehRigData.Parts)
            {
                var node = new GameObject(p.name).transform;
                Vector3 local;
                if (p.parent == null)
                {
                    node.SetParent(root, false);
                    local = new Vector3(0f, rig.baseHipY, 0f);
                    rig.torso = node;
                }
                else
                {
                    var pd = defs[p.parent];
                    node.SetParent(rig.nodes[p.parent], false);
                    local = new Vector3((p.parentJointX - pd.ownJointX) / rig.ppu, -(p.parentJointY - pd.ownJointY) / rig.ppu, 0f);
                }
                node.localPosition = local;

                var sg = new GameObject("sprite");
                sg.transform.SetParent(node, false);
                sg.transform.localPosition = new Vector3((p.w * 0.5f - p.ownJointX) / rig.ppu, (p.ownJointY - p.h * 0.5f) / rig.ppu, 0f);
                var sr = sg.AddComponent<SpriteRenderer>();
                sr.sprite = sprites[p.name];
                sr.sortingOrder = p.order;

                defs[p.name] = p;
                rig.nodes[p.name] = node;
            }
            return rig;
        }

        void Rot(string n, float deg)
        {
            Transform t;
            if (nodes.TryGetValue(n, out t)) t.localRotation = Quaternion.Euler(0f, 0f, deg);
        }

        /// <summary>هر فریم صدا زده شود. worldSpeed: سرعت افقی (مثبت)، reach: ۰ تا ۱ برای دراز کردن دست.</summary>
        public void Tick(float dt, float worldSpeed, float cycleLength, float reach)
        {
            clock += dt;
            walkWeight = Mathf.MoveTowards(walkWeight, worldSpeed > 0.15f ? 1f : 0f, dt * 6f);
            phase += (worldSpeed / Mathf.Max(0.1f, cycleLength)) * Mathf.PI * 2f * dt;

            float s = Mathf.Sin(phase), c = Mathf.Cos(phase);
            float sf = -s, cf = -c;
            float s2 = Mathf.Sin(2f * phase);

            // راه رفتن
            float wThighN = ThighSwing * s, wThighF = ThighSwing * sf;
            float wShinN = -KneeBend * Mathf.Max(0f, c), wShinF = -KneeBend * Mathf.Max(0f, cf);
            float wUarmN = -ArmSwing * s, wUarmF = -ArmSwing * sf;
            float wFarmN = 8f + 16f * Mathf.Max(0f, -s), wFarmF = 8f + 16f * Mathf.Max(0f, -sf);
            float wTorso = Lean + 1.2f * s2;
            float wHead = -1f * Mathf.Sin(2f * phase + 0.5f);
            float wPack = 1.5f * Mathf.Sin(2f * phase + 0.8f);

            // ایستاده (نفس کشیدن ظریف)
            float br = Mathf.Sin(clock * 1.7f);
            float iThighN = -3f, iThighF = 3f;
            float iUarmN = 3f, iUarmF = -3f, iFarm = 8f;
            float iTorso = 0.6f * br, iHead = 0.8f * Mathf.Sin(clock * 1.7f + 1f), iPack = 0f;

            float w = walkWeight;
            float thighN = Mathf.Lerp(iThighN, wThighN, w), thighF = Mathf.Lerp(iThighF, wThighF, w);
            float shinN = wShinN * w, shinF = wShinF * w;
            float uarmN = Mathf.Lerp(iUarmN, wUarmN, w), uarmF = Mathf.Lerp(iUarmF, wUarmF, w);
            float farmN = Mathf.Lerp(iFarm, wFarmN, w), farmF = Mathf.Lerp(iFarm, wFarmF, w);
            float torsoA = Mathf.Lerp(iTorso, wTorso, w);
            float headA = Mathf.Lerp(iHead, wHead, w);
            float packA = Mathf.Lerp(iPack, wPack, w);

            // دراز کردن دست (برداشتن چیزی از زمین)
            if (reach > 0.001f)
            {
                torsoA += -16f * reach;
                headA += -6f * reach;
                uarmN = Mathf.Lerp(uarmN, 40f, reach);
                farmN = Mathf.Lerp(farmN, 35f, reach);
                uarmF = Mathf.Lerp(uarmF, 10f, reach);
                thighN = Mathf.Lerp(thighN, 12f, reach);
                shinN = Mathf.Lerp(shinN, -18f, reach);
                thighF = Mathf.Lerp(thighF, -6f, reach);
            }

            Rot("thigh_near", thighN); Rot("thigh_far", thighF);
            Rot("shin_near", shinN); Rot("shin_far", shinF);
            Rot("uarm_near", uarmN); Rot("uarm_far", uarmF);
            Rot("farm_near", farmN); Rot("farm_far", farmF);
            Rot("torso", torsoA); Rot("head", headA); Rot("backpack", packA);

            // لگن هنگام باز بودن پاها کمی پایین می‌آید
            float drop = baseHipY * 0.9f * (1f - Mathf.Cos(ThighSwing * Mathf.Deg2Rad)) * s * s * w;
            drop += 0.04f * reach;
            torso.localPosition = new Vector3(0f, baseHipY - drop, 0f);
        }
    }
}
