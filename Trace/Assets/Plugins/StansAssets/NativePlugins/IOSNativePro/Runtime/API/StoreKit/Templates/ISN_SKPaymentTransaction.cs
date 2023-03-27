using System;
using UnityEngine;
using SA.Foundation.Templates;
using SA.Foundation.Time;
using SA.Foundation.Utility;

namespace SA.iOS.StoreKit 
{
    /// <inheritdoc cref="ISN_iSKPaymentTransaction" />
    [Serializable]
    public class ISN_SKPaymentTransaction : SA_Result, ISN_iSKPaymentTransaction
    {
        [SerializeField] string m_productIdentifier = null;
        [SerializeField] string m_transactionIdentifier = null;
        [SerializeField] private long m_unixDate = 0;
        [SerializeField] ISN_SKPaymentTransactionState m_state = default(ISN_SKPaymentTransactionState);
        [SerializeField] ISN_SKPaymentOriginalTransaction m_originalTransaction = null;


        public ISN_SKPaymentTransaction(ISN_SKProduct product, ISN_SKPaymentTransactionState state) 
        {
            m_productIdentifier = product.ProductIdentifier;
            m_transactionIdentifier = SA_IdFactory.RandomString;
            m_unixDate = SA_Unix_Time.ToUnixTime(DateTime.UtcNow);

            m_state = state;
        }

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

        public ISN_iSKPaymentTransaction OriginalTransaction {
            get {
                return m_originalTransaction;
            }
        }
    }
}