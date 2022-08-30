using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace TGC.MonoGame.TP.Content.Models
{
    class Car
    {
        /**   */
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        /**   */
        private Model CarModel { get; set; }
        private Model Model { get; set; }
        private Effect Effect { get; set; }
        private FollowCamera FollowCameraCar { get; set; }
        
        /** Variables de tiempo    */
        private float Timer {get; set; } =0f;
        public float Duracion {get; set; }= 1f;
        private float Rotation {get; set; }= 0f;
        public float Jump = 40f;
        private float Giro = 45f;
        /** Matrices y vectores   */
        private Vector3 Position {get; set; }=Vector3.Zero;
        private float Velocity { get; set; } = 1f;
        private Vector3 Acceleration { get; set; } = Vector3.Down * 350f;
        public Matrix World {get; private set; }
        public Matrix Vista {get;  set; }
        public Matrix Projetion {get;  set; }
        
        public bool OnGround { get; set; } =true;

        /** 
        * ? para mover de un lugar A a otro B 
        * ? Lerp(A,B,0)= A
        * ? Lerp (A,B,1)=B;
        * !  (1-C)*A + C*B  siendo C  entre 0 y 1
        */

        private Vector3 A {get; set; } = new Vector3(0f, 0f, 0f);
        private Vector3 B {get; set; } = new Vector3(0f, 0f, 5f);

        public void Inicializar(){
            World = Matrix.CreateTranslation(new Vector3(0f,1f,0f));
        }
        public Car(ContentManager content){
            Model = content.Load<Model>(ContentFolder3D + "scene/car");
            Effect = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            

              foreach (var mesh in Model.Meshes)
            {
                // A mesh contains a collection of parts
                foreach (var meshPart in mesh.MeshParts)
                    // Assign the loaded effect to each part
                    meshPart.Effect = Effect;
            }

        }
        public void Desacelero(GameTime gameTime){
            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            Timer -=elapsedTime;
            Velocity -=Timer*0.1f/3f;
                //Position += Vector3.Lerp(A,B,Velocity);
        }
        public void Acelero(GameTime gameTime){
            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            Timer +=elapsedTime;
            Velocity +=Timer*0.1f*2f;
               // Position += Vector3.Lerp(A,B,Velocity);
        }
        public void RotacionD(GameTime gameTime){
            Rotation -= Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
         //   Rotation-=Giro;
        }
        public void RotacionA(GameTime gameTime){
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
        }
        public void Salto(){
                if(OnGround){
                    Position += Vector3.Up * Jump;
                    OnGround =false;
                }
                else if(Position.Y >= 1f )
                {
                        Position -= Vector3.Down;
                    
                }
        }
        
        public void Update(GameTime gameTime){

            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            Timer +=elapsedTime;
            
                Position += Vector3.Forward * elapsedTime * Velocity; 
                World = 
                    Matrix.CreateScale(elapsedTime/ 0.5f)*
                    Matrix.CreateRotationY(Rotation)
                //  Matrix.CreateFromQuaternion(quaternion)
                    * Matrix.CreateTranslation(Position);

        } 
        public void Draw(GameTime gameTime, Matrix view, Matrix projection){
            // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando.
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
          //  Effect.Parameters["DiffuseColor"].SetValue(Color.DarkBlue.ToVector3());
           
            Matrix rotationMatrix = Matrix.CreateRotationY(Rotation);
            Matrix traslacionMatrix = Matrix.CreateTranslation(Position);

            foreach (var mesh in Model.Meshes)
            {
                World =mesh.ParentBone.Transform * rotationMatrix * traslacionMatrix;
            
                
                Effect.Parameters["World"].SetValue(World);
                mesh.Draw();
            }
          /*  foreach (var mesh in Model.Meshes)
            {
                World = mesh.ParentBone.Transform * traslacionMatrix;
                Effect.Parameters["World"].SetValue(World);
                mesh.Draw();
            }
            */
        } 
    }
}