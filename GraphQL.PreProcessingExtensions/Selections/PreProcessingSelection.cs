﻿# nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GraphQL.PreProcessingExtensions.Selections;

namespace HotChocolate.PreProcessingExtensions
{
    /// <summary>
    /// Adapter class to support mapping both ObjectTypes and InterfaceType 
    /// Field objects from HotChocolate to a set of common shared values; 
    /// primarily the GraphQL Field Name for Selection/Projection.
    /// </summary>
    public class PreProcessingSelection : IHasName, IPreProcessingSelection
    {
        public PreProcessingSelection(ObjectType objectType, IFieldSelection selectionField)
        {
            GraphQLObjectType = objectType ?? throw new ArgumentNullException(nameof(objectType));
            GraphQLFieldSelection = selectionField ?? throw new ArgumentNullException(nameof(selectionField));
            ClassMemberInfo = selectionField.Field?.Member;
        }

        public ObjectType GraphQLObjectType { get; }

        public IFieldSelection GraphQLFieldSelection { get; }

        public MemberInfo? ClassMemberInfo { get; }

        public string SelectionName => GraphQLFieldSelection.ResponseName.ToString();

        public string SelectionMemberName => ClassMemberInfo?.Name! ?? SelectionName;

        /// <summary>
        /// Select the MemberName if possible otherwise retrieve the SelectionName
        /// because technically the underlying IFieldSelection.Member is a nullable field.
        /// </summary>
        public string SelectionMemberNameOrDefault => ClassMemberInfo?.Name! ?? SelectionName;

        public NameString Name => GraphQLFieldSelection.ResponseName;
        
        public override string ToString()
        {
            return $"{GraphQLObjectType.Name}:{SelectionName}";
        }
    }
}
