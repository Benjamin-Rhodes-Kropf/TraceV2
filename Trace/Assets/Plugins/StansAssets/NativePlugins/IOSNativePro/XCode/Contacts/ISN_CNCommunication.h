#if !TARGET_OS_TV

#import "ISN_Foundation.h"

#import <Contacts/Contacts.h>
#import <ContactsUI/ContactsUI.h>

@protocol ISN_CNPhoneNumber;
@interface ISN_CNPhoneNumber : JSONModel
-(id) initWithCNLabeledValue:(CNLabeledValue *) phone;
@property (nonatomic) NSString *m_countryCode;
@property (nonatomic) NSString *m_digits;
@end


@protocol ISN_CNContact;
@interface ISN_CNContact : JSONModel
-(id) initWithContact:(CNContact *) contact;

@property (nonatomic) NSString *m_givenName;
@property (nonatomic) NSString *m_familyName;
@property (nonatomic) NSString *m_nickname;
@property (nonatomic) NSString *m_organizationName;
@property (nonatomic) NSString *m_departmentName;
@property (nonatomic) NSString *m_jobTitle;



@property (nonatomic) NSArray<NSString *> * m_emails;
@property (nonatomic) NSArray<ISN_CNPhoneNumber> * m_phones;
@end


@protocol ISN_CNContactsResult;
@interface ISN_CNContactsResult : SA_Result
@property (nonatomic) NSMutableArray<ISN_CNContact> * m_contacts;
-(void) addContact:(ISN_CNContact*) contact;
@end

#endif






