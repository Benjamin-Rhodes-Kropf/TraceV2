#if !TARGET_OS_TV

#import "ISN_CNCommunication.h"


@implementation ISN_CNPhoneNumber
-(id) init { return self = [super init]; }
-(id) initWithCNLabeledValue:(CNLabeledValue *) phone {
    self = [super init];
    if(self) {
        self.m_countryCode =[phone.value valueForKey:@"countryCode"];
        self.m_digits =[phone.value valueForKey:@"digits"];
    }
    return self;
}
@end



@implementation ISN_CNContact
-(id) init { return self = [super init]; }
-(id) initWithContact:(CNContact *) contact {
    self = [super init];
    if(self) {
        self.m_givenName = contact.givenName == NULL ? @"" : contact.givenName;
        self.m_familyName = contact.familyName       == NULL ? @"" : contact.familyName;
        self.m_nickname = contact.nickname       == NULL ? @"" : contact.nickname;
        self.m_organizationName = contact.organizationName       == NULL ? @"" : contact.organizationName;
        self.m_departmentName = contact.departmentName       == NULL ? @"" : contact.departmentName;
        self.m_jobTitle = contact.jobTitle       == NULL ? @"" : contact.jobTitle;
        
        
        
        NSMutableArray* emails = [[NSMutableArray alloc] init];
        for (CNLabeledValue* mail in  contact.emailAddresses) {
            [emails addObject:mail.value];
        }
        self.m_emails = emails;
        
        NSMutableArray* phones = [[NSMutableArray alloc] init];
        for (CNLabeledValue* phone in  contact.phoneNumbers) {
            
            ISN_CNPhoneNumber* phoneNumber = [[ISN_CNPhoneNumber alloc] initWithCNLabeledValue:phone];
            [phones addObject:phoneNumber];
        }
        self.m_phones = phones;
        
        
        
    }
    return self;
}
@end


@implementation ISN_CNContactsResult
-(id) init {
    self = [super init];
    if(self) {
        self.m_contacts = [[NSMutableArray alloc] init] ;
    }
    
    return self;
}
-(void) addContact:(ISN_CNContact*) contact {
    [self.m_contacts addObject:contact];
}
@end

#endif
