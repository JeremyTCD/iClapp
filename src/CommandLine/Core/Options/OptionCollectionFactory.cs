﻿using System.Collections.Generic;
using System.Reflection;

namespace JeremyTCD.DotNet.CommandLine
{ 
    public class OptionCollectionFactory : IOptionCollectionFactory
    {
        private readonly IDictionary<ICommand, OptionCollection> _optionCollectionCache = new Dictionary<ICommand, OptionCollection>();
        private readonly IOptionFactory _optionFactory;

        public OptionCollectionFactory(IOptionFactory optionFactory)
        {
            _optionFactory = optionFactory;
        }

        /// <inheritdoc/>
        public virtual IOptionCollection Create(ICommand command)
        {
            _optionCollectionCache.TryGetValue(command, out OptionCollection result);

            if (result == null)
            {
                result = new OptionCollection();
                foreach (PropertyInfo propertyInfo in command.GetType().GetProperties())
                {
                    IOption option = _optionFactory.TryCreate(propertyInfo);
                    if (option != null)
                    {
                        result.Add(option);
                    }
                }

                _optionCollectionCache.Add(command, result);
            }

            return result;
        }
    }
}
