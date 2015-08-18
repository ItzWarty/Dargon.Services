using Dargon.Services.Utilities;
using ItzWarty;
using ItzWarty.Collections;
using System;
using System.Data;
using System.Reflection;
using Dargon.Services.Messaging;

namespace Dargon.Services.Server {
   public interface InvokableServiceContext {
      Guid Guid { get; }
      object HandleInvocation(string action, object[] arguments);
      object HandleInvocation(string action, PortableObjectBox arguments);
   }

   public class InvokableServiceContextImpl : InvokableServiceContext {
      private readonly ICollectionFactory collectionFactory;
      private readonly PortableObjectBoxConverter portableObjectBoxConverter;
      private readonly object serviceImplementation;
      private readonly Type serviceInterface;
      private readonly Guid guid;
      private readonly IMultiValueDictionary<string, MethodInfo> methodsByName;

      public InvokableServiceContextImpl(ICollectionFactory collectionFactory, PortableObjectBoxConverter portableObjectBoxConverter, object serviceImplementation, Type serviceInterface, Guid guid, IMultiValueDictionary<string, MethodInfo> methodsByName) {
         this.collectionFactory = collectionFactory;
         this.portableObjectBoxConverter = portableObjectBoxConverter;
         this.serviceImplementation = serviceImplementation;
         this.serviceInterface = serviceInterface;
         this.guid = guid;
         this.methodsByName = methodsByName;
      }

      public Guid Guid => guid;

      public object HandleInvocation(string action, object[] arguments) {
         HashSet<MethodInfo> candidates;
         if (methodsByName.TryGetValue(action, out candidates)) {
            foreach (var candidate in candidates) {
               var parameters = candidate.GetParameters();
               if (parameters.Length != arguments.Length) {
                  continue;
               }
               return candidate.Invoke(serviceImplementation, arguments);
            }
         }
         throw new EntryPointNotFoundException("Could not find method " + action + " with given arguments");
      }

      public object HandleInvocation(string action, PortableObjectBox arguments) {
         object[] methodArguments;
         if (portableObjectBoxConverter.TryConvertFromDataTransferObject(arguments, out methodArguments)) {
            return HandleInvocation(action, methodArguments);
         } else {
            throw new PortableException(new Exception("Could not deserialize data in argument dto."));
         }
      }
   }
}