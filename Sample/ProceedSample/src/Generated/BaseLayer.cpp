#include "./Generated/API.h"
#include "./Generated/Layer_Private.h"
#include "./Generated/LayerdObject_Private.h"
#include "./Generated/DependentCode.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "RTCOP/Core/PartialClassMembers.h"
#include <cstring>

#include "./Generated/API.h"
#include "stdio.h"

#include "./Generated/BaseLayer.h"

namespace RTCOP {
namespace Generated {

BaseLayer* BaseLayer::GetInstance()
{
	return (BaseLayer*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::baselayer);
}

BaseLayer::BaseLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	:Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)
{
	int size0 = sizeof(volatile void*) * numOfBaseMethods[0];
	volatile void** virtualFunctionTable0 = DependentCode::baselayer::A::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
	int size1 = sizeof(volatile void*) * numOfBaseMethods[1];
	volatile void** virtualFunctionTable1 = DependentCode::baselayer::B::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[1], virtualFunctionTable1, size1);
}

BaseLayer::~BaseLayer()
{
}

void* BaseLayer::InitializeLayerdObject(void* obj, int classID)
{
	return obj;
}

} // namespace Generated {}
} // namespace RTCOP {}

using namespace RTCOP ;
namespace baselayer
{
A :: A ( ) 
{
b = copnew < baselayer :: B > ( ) ;

}

A :: ~ A ( ) 
{
delete b ;

}

void A::m1 ()
{
	printf ( "baselayer::A::m1()\n" ) ;
	b -> m2 ( ) ;

}

B :: B ( ) 
{

}

B :: ~ B ( ) 
{

}

void B::m2 ()
{
	printf ( "baselayer::B::m2()\n" ) ;

}


}


