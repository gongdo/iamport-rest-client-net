using Sample.AspNetCore.Models;
using System;
using System.Collections.Concurrent;

namespace Sample.AspNetCore.Repositories
{
    /// <summary>
    /// 구매 정보를 저장하는 리포지터리 샘플
    /// </summary>
    public class CheckoutRepository
    {
        private ConcurrentDictionary<Guid, Checkout> checkouts
            = new ConcurrentDictionary<Guid, Checkout>();

        /// <summary>
        /// 주어진 구매 정보를 새로 저장합니다.
        /// </summary>
        /// <param name="checkout">구매 정보</param>
        /// <exception cref="DuplicatedKeyException">해당 구매 정보의 Id가 이미 저장되어 있습니다.</exception>
        public void Add(Checkout checkout)
        {
            if (!checkouts.TryAdd(checkout.Id, checkout))
            {
                throw new DuplicatedKeyException($"Duplicated id {checkout.Id} detected.");
            }
        }

        /// <summary>
        /// 주어진 Id의 구매 정보를 반환합니다.
        /// 존재하지 않을 경우 null을 반환합니다.
        /// </summary>
        /// <param name="id">구매 정보의 Id</param>
        /// <returns>해당 Id의 주문 정보</returns>
        public Checkout GetById(Guid id)
        {
            Checkout checkout = null;
            checkouts.TryGetValue(id, out checkout);
            return checkout;
        }
    }
}
