import { Button, Card } from "semantic-ui-react";
import { Activity } from "../../../app/models/activity";

interface Props {
    activity: Activity;
    cancelSelectActivity: () => void;
    openForm: (id: string) => void;
    
}

export default function ActivityDetails({ activity, cancelSelectActivity, openForm }: Props) {
    
    return (
        <Card fluid>
            <img src={`/assets/categoryImages/${activity.category}.jpg`} alt={`${activity.category}`} />
            <Card.Content>
                <Card.Header>{activity.title}</Card.Header>
                <Card.Meta>
                    <span>{activity.date}</span>
                </Card.Meta>
                <Card.Description>
                    {activity.description}
                </Card.Description>
            </Card.Content>
            <Card.Content extra>
                <Button.Group>
                    <Button onClick={() => openForm(activity.id)} basic color="blue" content='Edit' />
                    <Button onClick={cancelSelectActivity} basic color="grey" content='Cancel'/>
                </Button.Group>
            </Card.Content>
        </Card>
    )
}