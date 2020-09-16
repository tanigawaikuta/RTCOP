#include "./Generated/API.h"
#include "./Generated/Layer_Private.h"
#include "./Generated/LayerdObject_Private.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "./Generated/DependentCode.h"
#include <iostream>
#include <cstring>

#include "stdio.h"

#include "./Generated/BaseLayer.h"
#include "./Generated/Layer1.h"

namespace RTCOP {
namespace Generated {

Layer1* Layer1::GetInstance()
{
	return (Layer1*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::Layer1);
}

Layer1::Layer1(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	:RTCOP::Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)
{
	int size0 = sizeof(volatile void*) * numOfBaseMethods[0];
	volatile void** virtualFunctionTable0	 = DependentCode::baselayer::Sample::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
	_Private->_VirtualFunctionTables[0][0] = 0;
}

Layer1::~Layer1()
{
}

void* Layer1::InitializeLayerdObject(void* obj, int classID)
{
	int layerID = _Private->_ID;
	if (classID == 0)
	{
		::Layer1::Sample* layerdObject = reinterpret_cast<::Layer1::Sample*>(obj);
		layerdObject->_Private->_PartialClassMembers[layerID] = new ::Layer1::Sample::PartialClassMembers();
		layerdObject->_Private->_PartialClassMembers[layerID]->_Layer = this;
		layerdObject->_Private->_PartialClassMembers[layerID]->_VirtualFunctionTableForProceeding = _Private->_VirtualFunctionTablesForProceeding[classID];
		volatile void* vfp = DependentCode::GetLayerdObjectFinalizer(layerdObject);
		layerdObject->_Private->_PartialClassMembers[layerID]->_Finalizer = vfp;
		layerdObject->_RTCOP_InitializePartialClass();
	}
	return obj;
}

void Layer1::_RTCOP_OnActivating()
{
	::Layer1::OnActivating();
}

void Layer1::_RTCOP_OnActivated()
{
	::Layer1::OnActivated();
}

void Layer1::_RTCOP_OnDeactivating()
{
	::Layer1::OnDeactivating();
}

void Layer1::_RTCOP_OnDeactivated()
{
	::Layer1::OnDeactivated();
}

} // namespace Generated {}
} // namespace RTCOP {}

namespace Layer1
{
void Sample::Initialize ()
{
	Sample::PartialClassMembers* layer_members = (Sample::PartialClassMembers*)_Private->_PartialClassMembers[1];
	printf ( "Layer1::Sample::Initializeの実行\n" ) ;

}
void Sample::Finalize ()
{
	Sample::PartialClassMembers* layer_members = (Sample::PartialClassMembers*)_Private->_PartialClassMembers[1];
	printf ( "Layer1::Sample::Finalizeの実行\n" ) ;

}
void OnActivating ()
{
	printf ( "Layer1::OnActivatingの実行\n" ) ;
	}

void OnActivated ()
{
	printf ( "Layer1::OnActivatedの実行\n" ) ;
	}

void OnDeactivating ()
{
	printf ( "Layer1::OnDeactivatingの実行\n" ) ;
	}

void OnDeactivated ()
{
	printf ( "Layer1::OnDeactivatedの実行\n" ) ;
	}


}


namespace Layer1{

Sample::Sample()
	: RTCOP::Core::LayerdObject<baselayer::Sample>()
{
}

void Sample::_RTCOP_InitializePartialClass()
{
	Initialize();
}

void Sample::_RTCOP_FinalizePartialClass()
{
	Finalize();
}


}

