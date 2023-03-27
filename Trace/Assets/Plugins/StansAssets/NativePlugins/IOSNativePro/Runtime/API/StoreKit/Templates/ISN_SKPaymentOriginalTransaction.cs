using System;
using UnityEngine;
using SA.Foundation.Templates;
using SA.Foundation.Time;

namespace SA.iOS.StoreKit 
{
    /// <inheritdoc />
    [Serializable]
    public class ISN_SKPaymentOriginalTransaction : ISN_iSKPaymentTransaction
    {
        [SerializeField] string m_productIdentifier = null;
        [SerializeField] string m_transactionIdentifier = null;
        [SerializeField] private long m_unixDate = 0;
        [SerializeField] ISN_SKPaymentTransactionState m_state = default(ISN_SKPaymentTransactionState);


        public string ProductIdentifier {
            get {
                return m_productIdentifier;
            }
        }

        public string TransactionIdentifier {
            get {
                return m_transactionIdentifier;
            }
        }

        public DateTime Date {
            get {
                return SA_Unix_Time.ToDateTime(m_unixDate);
            }
        }

        public ISN_SKPaymentTransactionState State {
            get {
                return m_state;
            }
        }

        public ISN_SKProduct Product {
            get {
                return ISN_SKPaymentQueue.GetProductById(m_productIdentifier);
            }
        }

        public ISN_iSKPaymentTransaction OriginalTransaction
        {
            get { return null; }
        }

        public void SetError(SA_Error error)
        {
            throw new NotImplementedException();
        }

        public string ToJson()
        {
            throw new NotImplementedException();
        }

        public SA_Error Error { get; }
        public bool HasError { get; }
        public bool IsSucceeded { get; }
        public bool IsFailed { get; }
        public string RequestId { get; }
    }
}