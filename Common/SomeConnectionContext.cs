﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dargon.PortableObjects.Streams;
using ItzWarty.Collections;
using ItzWarty.Networking;

namespace Dargon.Services.Common {
   public interface SomeConnectionContext : IDisposable {
      void HandleBundle(SomeConnectionFeatureBundle someConnectionFeatureBundle);
      void Start();
   }

   public class SomeConnectionContextImpl : SomeConnectionContext {
      private readonly IConnectedSocket socket;
      private readonly PofStream pofStream;
      private readonly PofDispatcher pofDispatcher;

      public SomeConnectionContextImpl(IConnectedSocket socket, PofStream pofStream, PofDispatcher pofDispatcher
      ) : this(socket, pofStream, pofDispatcher, new ConcurrentSet<SomeConnectionFeatureBundle>()){
      }

      public SomeConnectionContextImpl(IConnectedSocket socket, PofStream pofStream, PofDispatcher pofDispatcher, IConcurrentSet<SomeConnectionFeatureBundle> bundles) {
         this.socket = socket;
         this.pofStream = pofStream;
         this.pofDispatcher = pofDispatcher;
      }

      public void HandleBundle(SomeConnectionFeatureBundle someConnectionFeatureBundle) {
         someConnectionFeatureBundle.ConfigureDispatcher(pofDispatcher);
      }
      
      public void Start() {
         pofDispatcher.Start();
      }

      public void Dispose() {
         pofDispatcher.Dispose();
         pofStream.Dispose();
         socket.Dispose();
      }
   }
}