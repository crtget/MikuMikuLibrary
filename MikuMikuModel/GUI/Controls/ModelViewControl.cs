using MikuMikuLibrary.Maths;
using MikuMikuLibrary.Models;
using MikuMikuLibrary.Textures;
using MikuMikuModel.GUI.Controls.ModelView;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MikuMikuModel.GUI.Controls
{
    public partial class ModelViewControl : GLControl
    {
        private static ModelViewControl sInstance;

        public static ModelViewControl Instance => sInstance ?? (sInstance = new ModelViewControl());

        public static void DisposeInstance()
        {
            sInstance?.Dispose();
        }

        private static readonly float sCameraSpeed = 0.1f;
        private static readonly float sCameraSpeedFast = 0.8f;
        private static readonly float sCameraSpeedSlow = 0.025f;

        private static readonly Vector3 sCamUp = Vector3.UnitY;
        private static readonly float sFieldOfView = MathHelper.DegreesToRadians(65);

        private static readonly Color4 sClearColor = Color4.Black;

        private Stopwatch mUpdateStopwatch = Stopwatch.StartNew();
        private TimeSpan mElapsedTime;

        private float mElapsedFactor;
        private float mFramerate;
        private float mRefreshRate;

        private IGLDraw mModel;
        private GLShaderProgram mDefaultShader;
        private GLShaderProgram mGridShader;

        private Matrix4 mViewMatrix;
        private Matrix4 mProjectionMatrix;

        private Vector3 mCamPos = Vector3.Zero;
        private Vector3 mCamRot = new Vector3(-90, 0, 0);
        private Vector3 mCamDir = new Vector3(0, 0, -1);
        private Point mPrevMousePos;

        private bool mComputeProjectionMatrix = true;
        private bool mShouldRedraw = true;

        private bool mLeft, mRight, mUp, mDown;

        private int mGridVertexArrayId;
        private GLBuffer<Vector3> mGridVertexBuffer;

        public void SetModel(Model model, TextureSet textureSet)
        {
            if (mDefaultShader == null || mGridShader == null)
                return;

            mModel?.Dispose();
            mModel = new GLModel(model, textureSet);

            var bSphere = new BoundingSphere();
            foreach (var mesh in model.Meshes)
                bSphere.Merge(mesh.BoundingSphere);

            SetDefaultCamera(bSphere);
        }

        public void SetModel(Mesh mesh, TextureSet textureSet)
        {
            if (mDefaultShader == null || mGridShader == null)
                return;

            mModel?.Dispose();
            mModel = new GLMesh(mesh, new Dictionary<int, GLTexture>(), textureSet);

            SetDefaultCamera(mesh.BoundingSphere);
        }

        public void SetModel(SubMesh subMesh, Mesh mesh, TextureSet textureSet)
        {
            if (mDefaultShader == null || mGridShader == null)
                return;

            mModel?.Dispose();

            var materials = new List<GLMaterial>(new GLMaterial[mesh.Materials.Count]);
            var dictionary = new Dictionary<int, GLTexture>();

            foreach (var indexTable in subMesh.IndexTables)
            {
                if (materials[indexTable.MaterialIndex] == null)
                    materials[indexTable.MaterialIndex] = new GLMaterial(mesh.Materials[indexTable.MaterialIndex], dictionary, textureSet);
            }

            mModel = new GLSubMesh(subMesh, materials);

            SetDefaultCamera(subMesh.BoundingSphere);
        }

        public void ResetCamera()
        {
            mShouldRedraw = true;

            mCamPos = Vector3.Zero;
            mCamRot = new Vector3(-90, 0, 0);
            mCamDir = new Vector3(0, 0, -1);
        }

        private void SetDefaultCamera(BoundingSphere boundingSphere)
        {
            ResetCamera();

            var distance = (float)(boundingSphere.Radius * 2f / Math.Tan(sFieldOfView)) + 0.5f;

            mCamPos = new Vector3(
                boundingSphere.Center.X,
                boundingSphere.Center.Y,
                boundingSphere.Center.Z + distance);
        }

        private void UpdateCameraInput()
        {
            float x = MathHelper.DegreesToRadians(mCamRot.X);
            float y = MathHelper.DegreesToRadians(mCamRot.Y);
            float yCos = (float)Math.Cos(y);

            var front = new Vector3()
            {
                X = (float)Math.Cos(x) * yCos,
                Y = (float)Math.Sin(y),
                Z = (float)Math.Sin(x) * yCos
            };

            mCamDir = Vector3.Normalize(front);

            float cameraSpeed = ModifierKeys.HasFlag(Keys.Shift) ? sCameraSpeedFast : ModifierKeys.HasFlag(Keys.Control) ? sCameraSpeedSlow : sCameraSpeed;
            cameraSpeed /= mElapsedFactor;

            if (mUp && !mDown)
                mCamPos += mCamDir * cameraSpeed;
            if (mDown && !mUp)
                mCamPos -= mCamDir * cameraSpeed;

            if (mLeft && !mRight)
                mCamPos -= Vector3.Normalize(Vector3.Cross(mCamDir, sCamUp)) * cameraSpeed;
            if (mRight && !mLeft)
                mCamPos += Vector3.Normalize(Vector3.Cross(mCamDir, sCamUp)) * cameraSpeed;

            if (mUp || mDown || mLeft || mRight)
                mShouldRedraw = true;
        }

        private Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(mCamPos, mCamPos + mCamDir, sCamUp);
        }

        private Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(sFieldOfView, (float)Width / Height, 0.1f, 1000000f);
        }

        private List<DisplayDevice> GetDisplayDevices()
        {
            List<DisplayDevice> displayDevices = new List<DisplayDevice>((int)DisplayIndex.Sixth + 1);

            for (var display = DisplayIndex.First; display <= DisplayIndex.Sixth; display++)
            {
                var device = DisplayDevice.GetDisplay(display);
                if (device != null)
                    displayDevices.Add(device);
            }

            return displayDevices;
        }

        protected override void OnLoad(EventArgs arg)
        {
            Application.Idle += OnApplicationIdle;
            mRefreshRate = GetDisplayDevices().Max(d => d.RefreshRate);

            CreateGrid();

            base.OnLoad(arg);
        }

        private void OnApplicationIdle(object sender, EventArgs e)
        {
            Invalidate();
        }

        private Vector3 RotatePoint(Vector3 point, Vector3 origin, Vector3 angles)
        {
            return Quaternion.FromEulerAngles(angles) * (point - origin) + origin;
        }

        private void CreateGrid()
        {
            var vertices = new List<Vector3>();

            for (float i = -10; i <= 10; i += 0.5f)
            {
                vertices.Add(new Vector3(i, 0, -10));
                vertices.Add(new Vector3(i, 0, 10));
                vertices.Add(new Vector3(-10, 0, i));
                vertices.Add(new Vector3(10, 0, i));
            }

            mGridVertexArrayId = GL.GenVertexArray();
            GL.BindVertexArray(mGridVertexArrayId);

            mGridVertexBuffer = new GLBuffer<Vector3>(BufferTarget.ArrayBuffer, vertices.ToArray(), 12, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, mGridVertexBuffer.Stride, 0);
            GL.EnableVertexAttribArray(0);
        }

        private void UpdateInput()
        {
            UpdateCameraInput();
        }

        private void DrawModel(ref Matrix4 view, ref Matrix4 projection)
        {
            mDefaultShader.Use();
            mDefaultShader.SetUniform("view", view);
            mDefaultShader.SetUniform("projection", projection);
            mDefaultShader.SetUniform("viewPosition", mCamPos);
            mDefaultShader.SetUniform("lightPosition", mCamPos);
            mModel.Draw(mDefaultShader);
        }

        private void DrawGrid(ref Matrix4 view, ref Matrix4 projection)
        {
            mGridShader.Use();
            mGridShader.SetUniform("view", view);
            mGridShader.SetUniform("projection", projection);
            mGridShader.SetUniform("color", new Vector4(0.15f, 0.15f, 0.15f, 1f));

            GL.BindVertexArray(mGridVertexArrayId);
            GL.DrawArrays(PrimitiveType.Lines, 0, mGridVertexBuffer.Length);
        }

        private void DrawScene()
        {
            if (mComputeProjectionMatrix)
                mProjectionMatrix = GetProjectionMatrix();

            mViewMatrix = GetViewMatrix();

            if (mModel != null && mDefaultShader != null)
                DrawModel(ref mViewMatrix, ref mProjectionMatrix);

            if (mGridShader != null)
                DrawGrid(ref mViewMatrix, ref mProjectionMatrix);
        }

        private void MeasureElapsedTime()
        {
            mElapsedTime = mUpdateStopwatch.ElapsedMilliseconds == 0 ? mUpdateStopwatch.Elapsed : TimeSpan.FromMilliseconds(1);
            mUpdateStopwatch.Restart();

            mFramerate = (float)(1.0 / mElapsedTime.TotalSeconds);
            mElapsedFactor = (float)(mFramerate / 1.0 / 60.0);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            MeasureElapsedTime();

            if (Focused)
                UpdateInput();

            if (mShouldRedraw)
            {
                mShouldRedraw = false;

                GL.ClearColor(sClearColor);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                {
                    DrawScene();
                }
                SwapBuffers();
            }

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1.0 / (mRefreshRate * 1.3)));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            float deltaX = e.Location.X - mPrevMousePos.X;
            float deltaY = e.Location.Y - mPrevMousePos.Y;

            if (e.Button == MouseButtons.Left)
            {
                float cameraSpeed = ModifierKeys.HasFlag(Keys.Shift) ? 0.04f : ModifierKeys.HasFlag(Keys.Control) ? 0.00125f : 0.005f;
                cameraSpeed /= mElapsedFactor;

                var dirRight = Vector3.Normalize(Vector3.Cross(mCamDir, sCamUp));
                var dirUp = Vector3.Normalize(Vector3.Cross(mCamDir, dirRight));
                mCamPos -= ((dirRight * deltaX) + (dirUp * deltaY)) * cameraSpeed;
            }
            else if (e.Button == MouseButtons.Right)
            {
                mCamRot.X += deltaX * 0.1f;
                mCamRot.Y -= deltaY * 0.1f;
            }

            if (e.Button != MouseButtons.None)
                mShouldRedraw = true;

            mPrevMousePos = e.Location;

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            float cameraSpeed = ModifierKeys.HasFlag(Keys.Shift) ? 0.04f : ModifierKeys.HasFlag(Keys.Control) ? 0.00125f : 0.005f;
            mCamPos += mCamDir * cameraSpeed * e.Delta;

            mShouldRedraw = true;
            base.OnMouseWheel(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            bool keyHandled = true;

            switch (e.KeyCode)
            {
                case Keys.W:
                    mUp = true;
                    break;

                case Keys.A:
                    mLeft = true;
                    break;

                case Keys.S:
                    mDown = true;
                    break;

                case Keys.D:
                    mRight = true;
                    break;

                default:
                    keyHandled = false;
                    break;
            }

            if (keyHandled)
                e.Handled = true;

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            bool keyHandled = true;

            switch (e.KeyCode)
            {
                case Keys.W:
                    mUp = false;
                    break;

                case Keys.A:
                    mLeft = false;
                    break;

                case Keys.S:
                    mDown = false;
                    break;

                case Keys.D:
                    mRight = false;
                    break;

                default:
                    keyHandled = false;
                    break;
            }

            if (keyHandled)
                e.Handled = true;

            base.OnKeyUp(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            mUp = mLeft = mDown = mRight = false;
            base.OnLostFocus(e);
        }

        protected override void OnResize(EventArgs e)
        {
            mShouldRedraw = true;
            mComputeProjectionMatrix = true;

            base.OnResize(e);

            GL.Viewport(ClientRectangle);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                mComponents?.Dispose();
                mDefaultShader?.Dispose();
                mGridShader?.Dispose();
                mModel?.Dispose();
                mGridVertexBuffer?.Dispose();

                Application.Idle -= OnApplicationIdle;
            }

            GL.DeleteVertexArray(mGridVertexArrayId);
            base.Dispose(disposing);
        }

        ~ModelViewControl()
        {
            Dispose(false);
        }

        private ModelViewControl() : base(new GraphicsMode(32, 24, 0, 4), 3, 3, GraphicsContextFlags.ForwardCompatible)
        {
            InitializeComponent();
            MakeCurrent();
            VSync = false;

            mDefaultShader = GLShaderProgram.Create("Default");
            if (mDefaultShader == null)
                mDefaultShader = GLShaderProgram.Create("DefaultBasic");

            mGridShader = GLShaderProgram.Create("Grid");
            if (mDefaultShader == null || mGridShader == null)
            {
                Debug.WriteLine("Shader compilation failed. GL rendering will be disabled.");

                Visible = false;
                return;
            }

            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.PrimitiveRestartFixedIndex);
            GL.Enable(EnableCap.FramebufferSrgb);
        }
    }
}
