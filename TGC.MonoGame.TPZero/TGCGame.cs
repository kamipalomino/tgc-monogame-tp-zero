using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Content.Models;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Clase principal del juego.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";
        
        private GraphicsDeviceManager Graphics { get; }
        private CityScene City { get; set; }
        
        private Model CarModel { get; set; }
        private Matrix CarWorld { get; set; }
        private FollowCamera FollowCamera { get; set; }
        public bool OnGround {get; set; }
        private float Timer {get; set; } =0f;
        public float Duracion {get; set; }= 1f;
        private float Rotation {get; set; }= 0f;
        public float Jump = 40f;
        private float Giro = 45f;
        /** Matrices y vectores   */
        private Vector3 Position {get; set; }=Vector3.Zero;
        private float Velocity { get; set; } = 1f;
        private Vector3 Acceleration { get; set; } = Vector3.Down * 350f;

        /// <summary>
        ///     Constructor del juego
        /// </summary>
        public TGCGame()
        {
            // Se encarga de la configuracion y administracion del Graphics Device
            Graphics = new GraphicsDeviceManager(this);

            // Carpeta donde estan los recursos que vamos a usar
            Content.RootDirectory = "Content";

            // Hace que el mouse sea visible
            IsMouseVisible = true;
        }

        /// <summary>
        ///     Llamada una vez en la inicializacion de la aplicacion.
        ///     Escribir aca todo el codigo de inicializacion: Todo lo que debe estar precalculado para la aplicacion.
        /// </summary>
        protected override void Initialize()
        {
            // Enciendo Back-Face culling
            // Configuro Blend State a Opaco
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;

            // Configuro el tamaño de la pantalla
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            Graphics.ApplyChanges();

            // Creo una camara para seguir a nuestro auto
        
            FollowCamera = new FollowCamera(GraphicsDevice.Viewport.AspectRatio);

            // Configuro la matriz de mundo del auto
            CarWorld = Matrix.Identity;
            OnGround =true;
            base.Initialize();
        }

        /// <summary>
        ///     Llamada una sola vez durante la inicializacion de la aplicacion, luego de Initialize, y una vez que fue configurado GraphicsDevice.
        ///     Debe ser usada para cargar los recursos y otros elementos del contenido.
        /// </summary>
        protected override void LoadContent()
        {
            // Creo la escena de la ciudad
            City = new CityScene(Content);
            CarModel  = Content.Load<Model>(ContentFolder3D + "scene/car");
            
            // La carga de contenido debe ser realizada aca
        

            base.LoadContent();
        }

        /// <summary>
        ///     Es llamada N veces por segundo. Generalmente 60 veces pero puede ser configurado.
        ///     La logica general debe ser escrita aca, junto al procesamiento de mouse/teclas
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Caputo el estado del teclado
            var keyboardState = Keyboard.GetState();
          //  var keyboardKey = Keyboard.Get
            if (keyboardState.IsKeyDown(Keys.Q))
                // Salgo del juego
                Exit();

            // La logica debe ir aca
            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            Position += CarWorld.Forward * elapsedTime * Velocity;
            /*
            *! <kbd>w</kbd> y <kbd>s</kbd> para acelerar y desacelerar
            *! <kbd>a</kbd> y <kbd>d</kbd> para girar el auto
            */
            if (keyboardState.IsKeyDown(Keys.W))
                Acelero(elapsedTime);
            if (keyboardState.IsKeyDown(Keys.S))
                Desacelero(elapsedTime);
            if (keyboardState.IsKeyDown(Keys.A) && OnGround)
                RotacionA(elapsedTime);
            if (keyboardState.IsKeyDown(Keys.D) && OnGround)
                RotacionD(elapsedTime);
            if (keyboardState.IsKeyDown(Keys.Space) && OnGround){
               Salto();
               /*  if(OnGround){
                    Salto();
                }else if(Duracion < Automovil.Jump){
                    Salto();
                    Duracion+=1f;
                }
                OnGround=true;
                Duracion=1f;
 */            }
                 CarWorld =
                    Matrix.CreateRotationY(Rotation)
                //  Matrix.CreateFromQuaternion(quaternion)
                    * Matrix.CreateTranslation(Position);


            // Actualizo la camara, enviandole la matriz de mundo del auto
            FollowCamera.Update(gameTime, CarWorld);
  
            base.Update(gameTime);
        }


        /// <summary>
        ///     Llamada para cada frame
        ///     La logica de dibujo debe ir aca.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Limpio la pantalla
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Dibujo la ciudad
            City.Draw(gameTime, FollowCamera.View, FollowCamera.Projection);

            // El dibujo del auto debe ir aca
            CarModel.Draw(CarWorld,FollowCamera.View,FollowCamera.Projection);
            
            base.Draw(gameTime);
        }

        /// <summary>
        ///     Libero los recursos cargados
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos cargados dessde Content Manager
            Content.Unload();

            base.UnloadContent();
        }
        public void Desacelero(float elapsedTime){
            Timer -=elapsedTime;
            Velocity -=Timer*0.1f/3f;
                //Position += Vector3.Lerp(A,B,Velocity);
        }
        public void Acelero(float elapsedTime){
            
            Timer +=elapsedTime;
            Velocity +=Timer*0.1f*2f;
               // Position += Vector3.Lerp(A,B,Velocity);
        }
        public void RotacionD(float elapsedTime){
            Rotation -=elapsedTime;
         //   Rotation-=Giro;
        }
        public void RotacionA(float elapsedTime){
            Rotation += elapsedTime;
        }
        public void Salto(){
                    Position += CarWorld.Up *Jump;
                    OnGround =false;
                
        }
        
    }
}