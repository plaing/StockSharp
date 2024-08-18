﻿#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.BusinessEntities.BusinessEntities
File: IExchangeInfoProvider.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Algo.Storages
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ecng.Collections;

	using StockSharp.BusinessEntities;

	/// <summary>
	/// The storage based provider of stocks and trade boards.
	/// </summary>
	public class StorageExchangeInfoProvider : InMemoryExchangeInfoProvider
	{
		private readonly IEntityRegistry _entityRegistry;

		/// <summary>
		/// Initializes a new instance of the <see cref="StorageExchangeInfoProvider"/>.
		/// </summary>
		/// <param name="entityRegistry">The storage of trade objects.</param>
		/// <param name="autoInit">Invoke <see cref="Init"/> method.</param>
		public StorageExchangeInfoProvider(IEntityRegistry entityRegistry, bool autoInit = true)
		{
			_entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));

			if (autoInit)
				Init();
		}

		/// <inheritdoc />
		public override void Init()
		{
			var boardCodes = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

			boardCodes.AddRange(_entityRegistry.ExchangeBoards.Select(b => b.Code));

			var boards = Boards.Where(b => !boardCodes.Contains(b.Code)).ToArray();

			if (boards.Length > 0)
			{
				boards
					.Select(b => b.Exchange)
					.Distinct()
					.ForEach(Save);

				boards
					.ForEach(Save);
			}

			_entityRegistry.Exchanges.ForEach(e => base.Save(e));
			_entityRegistry.ExchangeBoards.ForEach(b => base.Save(b));

			base.Init();
		}

		/// <inheritdoc />
		public override void Save(ExchangeBoard board)
		{
			if (board == null)
				throw new ArgumentNullException(nameof(board));

			_entityRegistry.ExchangeBoards.Save(board);

			base.Save(board);
		}

		/// <inheritdoc />
		public override void Save(Exchange exchange)
		{
			if (exchange == null)
				throw new ArgumentNullException(nameof(exchange));

			_entityRegistry.Exchanges.Save(exchange);

			base.Save(exchange);
		}
	}
}