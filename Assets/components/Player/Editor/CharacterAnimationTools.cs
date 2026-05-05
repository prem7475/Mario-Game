using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MarioGame.Components.Player.Editor
{
    public static class CharacterAnimationTools
    {
        private const string AnimRoot = "Assets/assets/animations/Player";
        private const string SpriteRoot = "Assets/assets/sprites/Player";
        private const string ControllerPath = "Assets/assets/animations/Player/Player.controller";

        [MenuItem("MarioGame/Visuals/Create Player Animator Controller")]
        public static void CreatePlayerAnimator()
        {
            Directory.CreateDirectory(AnimRoot);
            Directory.CreateDirectory(SpriteRoot);

            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            }

            EnsureParams(controller);

            // Create/refresh clips (from sliced sprites if provided).
            var idle = CreateClip("Idle", LoadSprites("Idle"), 8);
            var run = CreateClip("Run", LoadSprites("Run"), 12);
            var jump = CreateClip("Jump", LoadSprites("Jump"), 10);
            var fall = CreateClip("Fall", LoadSprites("Fall"), 10);
            var bigIdle = CreateClip("BigIdle", LoadSprites("BigIdle"), 8);

            var sm = controller.layers[0].stateMachine;
            sm.states = new ChildAnimatorState[0];
            sm.anyStateTransitions = new AnimatorStateTransition[0];

            var sIdle = sm.AddState("Idle");
            sIdle.motion = idle;
            sIdle.writeDefaultValues = true;

            var sRun = sm.AddState("Run");
            sRun.motion = run;
            sRun.writeDefaultValues = true;

            var sJump = sm.AddState("Jump");
            sJump.motion = jump;
            sJump.writeDefaultValues = true;

            var sFall = sm.AddState("Fall");
            sFall.motion = fall;
            sFall.writeDefaultValues = true;

            var sBigIdle = sm.AddState("BigIdle");
            sBigIdle.motion = bigIdle;
            sBigIdle.writeDefaultValues = true;

            sm.defaultState = sIdle;

            // Transitions:
            // Jump/Fall based on Grounded + vertical velocity proxy (we use Jump bool; user can refine later)
            AddTransition(sIdle, sRun, "Speed", 0.1f, greater: true);
            AddTransition(sRun, sIdle, "Speed", 0.1f, greater: false);

            var tIdleJump = sIdle.AddTransition(sJump);
            tIdleJump.hasExitTime = false;
            tIdleJump.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");

            var tRunJump = sRun.AddTransition(sJump);
            tRunJump.hasExitTime = false;
            tRunJump.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");

            var tJumpFall = sJump.AddTransition(sFall);
            tJumpFall.hasExitTime = true;
            tJumpFall.exitTime = 0.75f;
            tJumpFall.duration = 0.05f;

            var tFallLand = sFall.AddTransition(sIdle);
            tFallLand.hasExitTime = false;
            tFallLand.AddCondition(AnimatorConditionMode.If, 0, "Grounded");

            // BigIdle: simple branch using Jump bool as a placeholder parameter "Big" isn't in controller by default
            // Users can extend with a real "Big" bool if they want animation changes.

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = controller;
        }

        private static void EnsureParams(AnimatorController controller)
        {
            EnsureParam(controller, "Speed", AnimatorControllerParameterType.Float);
            EnsureParam(controller, "Grounded", AnimatorControllerParameterType.Bool);
            EnsureParam(controller, "Jump", AnimatorControllerParameterType.Bool);
        }

        private static void EnsureParam(AnimatorController c, string name, AnimatorControllerParameterType type)
        {
            foreach (var p in c.parameters)
            {
                if (p.name == name)
                    return;
            }

            c.AddParameter(name, type);
        }

        private static void AddTransition(AnimatorState from, AnimatorState to, string param, float threshold, bool greater)
        {
            var t = from.AddTransition(to);
            t.hasExitTime = false;
            t.duration = 0.06f;
            t.AddCondition(greater ? AnimatorConditionMode.Greater : AnimatorConditionMode.Less, threshold, param);
        }

        private static AnimationClip CreateClip(string name, Sprite[] frames, int fps)
        {
            var path = $"{AnimRoot}/{name}.anim";
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
            {
                clip = new AnimationClip();
                AssetDatabase.CreateAsset(clip, path);
            }

            clip.frameRate = fps;

            var binding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = "",
                propertyName = "m_Sprite"
            };

            var keyframes = new List<ObjectReferenceKeyframe>();
            if (frames == null || frames.Length == 0)
            {
                // Fallback to single empty keyframe (user can replace later).
                keyframes.Add(new ObjectReferenceKeyframe { time = 0f, value = null });
            }
            else
            {
                for (var i = 0; i < frames.Length; i++)
                {
                    keyframes.Add(new ObjectReferenceKeyframe
                    {
                        time = i / (float)fps,
                        value = frames[i]
                    });
                }
            }

            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes.ToArray());
            EditorUtility.SetDirty(clip);
            return clip;
        }

        private static Sprite[] LoadSprites(string clipName)
        {
            // Put sprite sheets (sliced) under:
            // Assets/assets/sprites/Player/<clipName>/*.png
            var folder = $"{SpriteRoot}/{clipName}";
            if (!AssetDatabase.IsValidFolder(folder))
                return new Sprite[0];

            var guids = AssetDatabase.FindAssets("t:Sprite", new[] { folder });
            var sprites = new List<Sprite>();
            for (var i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (s != null) sprites.Add(s);
            }

            // Stable ordering: by name
            sprites.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
            return sprites.ToArray();
        }
    }
}

